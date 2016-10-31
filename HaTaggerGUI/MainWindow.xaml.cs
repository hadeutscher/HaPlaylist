/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using HaSyntaxLib;
using HaTagLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HaTaggerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data data;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = data = new Data();
        }

        public void SetFocusItem(LoadedFile focus_destination)
        {
            fileView.FocusedItem = null;
            fileView.FocusedItem = focus_destination;
        }

        private void fileView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                // Search for focus destination
                int old_index = fileView.SelectedIndex;
                LoadedFile focus_destination = null;

                // Search downward
                for (int i = fileView.SelectedIndex + 1; i < data.LoadedFiles.Count; i++)
                {
                    LoadedFile item = data.LoadedFiles[i];
                    if (!fileView.SelectedItems.Contains(item))
                    {
                        focus_destination = item;
                        break;
                    }
                }

                // If failed, search upward
                if (focus_destination == null)
                {
                    for (int i = fileView.SelectedIndex - 1; i >= 0; i--)
                    {
                        LoadedFile item = data.LoadedFiles[i];
                        if (!fileView.SelectedItems.Contains(item))
                        {
                            focus_destination = item;
                            break;
                        }
                    }
                }

                // We use ToList to copy the list before enumerating
                data.RemoveFiles(fileView.SelectedItems.Cast<LoadedFile>().ToList());

                // Set focused item
                fileView.SelectedIndex = data.LoadedFiles.IndexOf(focus_destination);
                SetFocusItem(focus_destination);
                
                e.Handled = true;
            }
        }

        private List<HaSyntax.HaSyntaxElement> parseAndValidateTags()
        {
            List<HaSyntax.HaSyntaxElement> tags = tagsBox.Text.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => new HaSyntax.HaSyntaxElement(x)).ToList();
            if (tags.Any(x => x.Element.Length == 0))
            {
                MessageBox.Show("You cannot input empty tags.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            else
            {
                return tags;
            }
        }

        private void applyTagsToList(List<HaSyntax.HaSyntaxElement> tags, IEnumerable<LoadedFile> list)
        {
            // Set tags
            foreach (LoadedFile file in list)
            {
                using (IHaTagger ht = TaggerFactory.CreateTagger(file.Filename, true))
                {
                    foreach (HaSyntax.HaSyntaxElement tag in tags)
                    {
                        if (tag.Flag)
                        {
                            ht.AddTag(tag.Element);
                        }
                        else
                        {
                            ht.RemoveTag(tag.Element);
                        }
                    }
                    ht.Save();
                }
            }

            // Reload views
            foreach (LoadedFile file in list)
            {
                file.Reload();
            }
        }

        private void updateAllButton_Click(object sender, RoutedEventArgs e)
        {
            List<HaSyntax.HaSyntaxElement> tags = parseAndValidateTags();
            if (tags != null)
            {
                applyTagsToList(tags, data.LoadedFiles);
            }
        }

        private void updateSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            List<HaSyntax.HaSyntaxElement> tags = parseAndValidateTags();
            if (tags != null)
            {
                applyTagsToList(tags, fileView.SelectedItems.Cast<LoadedFile>());
            }
        }
    }
}
