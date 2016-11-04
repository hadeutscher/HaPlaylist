/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Content.PM;
using System.Collections.Generic;
using Android.Database;
using System.IO;
using Java.Interop;
using HaTagLib;
using Android.Widget;
using System.Linq;
using HaSyntaxLib;
using Android.Net;
using System;
using Android.Annotation;
using Android;
using Android.Runtime;
using Android.Text;
using Android.Provider;
using Android.Media;
using System.Threading;
using Android.Webkit;

namespace HaPlaylist
{
    [Activity(Label = "HaPlaylist", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string SETTINGS_NAME = "hasettings";
        private const string SPINNER_NAMES = "spinner_names";
        private const string SPINNER_VALUES = "spinner_values";
        private const string FREE_TEXT_QUERY = "free_text_query";
        private const string SELECTED_INDEX = "selected_index";

        private List<Tuple<string, string>> saved_templates = new List<Tuple<string, string>>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            FindViewById<Spinner>(Resource.Id.templateSpinner).ItemSelected += templateSpinner_ItemSelected;
            FindViewById<EditText>(Resource.Id.queryInput).TextChanged += queryInput_TextChanged;

            RequestPermissions();
            LoadTemplates();
            LoadConfig();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SaveConfig();
        }

        private void UpdateTemplateSpinner()
        {
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, saved_templates.Select(x => x.Item1).ToList());
            FindViewById<Spinner>(Resource.Id.templateSpinner).Adapter = adapter;
        }

        private void LoadTemplates()
        {
            using (TemplatesDB db = new TemplatesDB(this))
            {
                saved_templates = db.GetData();
            }
            UpdateTemplateSpinner();
        }

        private void AddTemplate(string name, string value)
        {
            if (saved_templates.Any(x => x.Item1 == name))
            {
                throw new TemplateExistsException();
            }
            using (TemplatesDB db = new TemplatesDB(this))
            {
                db.AddData(name, value);
            }
            saved_templates.Add(new Tuple<string, string>(name, value));
            UpdateTemplateSpinner();
        }

        private void RemoveTemplate(string name)
        {
            using (TemplatesDB db = new TemplatesDB(this))
            {
                db.RemoveData(name);
            }
            saved_templates.RemoveAll(x => x.Item1 == name);
            UpdateTemplateSpinner();
        }

        private void LoadConfig()
        {
            ISharedPreferences prefs = GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private);
            string text = prefs.GetString(FREE_TEXT_QUERY, null);
            int selected_idx = prefs.GetInt(SELECTED_INDEX, -1);
            if (selected_idx != -1)
            {
                FindViewById<Spinner>(Resource.Id.templateSpinner).SetSelection(selected_idx);
            }
            if (text != null)
            {
                changeQueryInputSilent(text);
            }
        }

        private void SaveConfig()
        {
            using (ISharedPreferencesEditor editor = GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private).Edit())
            {
                editor.PutInt(SELECTED_INDEX, FindViewById<Spinner>(Resource.Id.templateSpinner).SelectedItemPosition);
                editor.PutString(FREE_TEXT_QUERY, FindViewById<EditText>(Resource.Id.queryInput).Text);
                editor.Commit();
            }
        }

        // Android GUI system is shit
        private bool internal_edit = false;
        private void changeQueryInputSilent(string text)
        {
            internal_edit = true;
            try
            {
                FindViewById<EditText>(Resource.Id.queryInput).Text = text;
            }
            finally
            {
                internal_edit = false;
            }
        }

        private void templateSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position > 0)
            {
                changeQueryInputSilent(saved_templates[e.Position].Item2);
            }
        }

        private void queryInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!internal_edit)
                FindViewById<Spinner>(Resource.Id.templateSpinner).SetSelection(0);
        }

        [Export("powerampButtonClick")]
        public void powerampButtonClick(View v)
        {
            createPlaylist(true);
            startPoweramp();
        }

        [Export("otherPlayersButtonClick")]
        public void otherPlayersButtonClick(View v)
        {
            createPlaylist(false);
            startViewIntent();
        }
        
        [Export("saveTemplateClick")]
        public void saveTemplateClick(View v)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Name");
            EditText text = new EditText(this);
            builder.SetView(text);
            AlertDialog dialog = null;
            builder.SetPositiveButton("OK", (s, e) =>
            {
                try
                {
                    AddTemplate(text.Text, FindViewById<EditText>(Resource.Id.queryInput).Text);
                }
                catch (TemplateExistsException)
                {
                    AlertDialog.Builder error_builder = new AlertDialog.Builder(this);
                    error_builder.SetTitle("Error");
                    error_builder.SetMessage("A template with this name already exists.");
                    error_builder.Show();
                }
                UpdateTemplateSpinner();
                FindViewById<Spinner>(Resource.Id.templateSpinner).SetSelection(saved_templates.Count - 1);
            });
            builder.SetNegativeButton("Cancel", (s, e) =>
            {
                dialog.Dismiss();
            });
            dialog = builder.Create();
            dialog.Window.SetSoftInputMode(SoftInput.StateVisible);
            dialog.Show();
        }

        private string getMusicFolderPath()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
        }

        private string getPlaylistPath(bool utf8)
        {
            return Path.Combine(getMusicFolderPath(), utf8 ? "haplaylist.m3u8" : "haplaylist.m3u");
        }

        private Android.Net.Uri uriFromPath(string path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }

        private void AddPlaylistToIndex(bool utf8)
        {
            string name = utf8 ? "haplaylist8" : "haplaylist";
            ContentResolver.Delete(MediaStore.Audio.Playlists.ExternalContentUri, "name == ?", new string[] { name });
            ContentValues cv = new ContentValues();
            cv.Put("name", utf8);
            cv.Put("_data", getPlaylistPath(utf8));
            cv.Put("date_added", DateTime.Now.ToFileTime());
            cv.Put("date_modified", DateTime.Now.ToFileTime());
            ContentResolver.Insert(MediaStore.Audio.Playlists.ExternalContentUri, cv);
        }

        private void createPlaylist(bool utf8)
        {
            HaSyntax syntax = new HaSyntax(FindViewById<EditText>(Resource.Id.queryInput).Text);
            string music_folder = getMusicFolderPath();
            string playlist_path = getPlaylistPath(utf8);
            using (StreamWriter sw = File.CreateText(playlist_path))
            {
                foreach (string file in Utils.ExpandDirectories(new string[] { music_folder }))
                {
                    try
                    {
                        using (IHaTagger ht = TaggerFactory.CreateTagger(file, false))
                        {
                            if (syntax.Validate(ht.Tags))
                            {
                                sw.WriteLine(file);
                            }
                        }
                    }
                    catch (HaException) { }
                }
            }
            AddPlaylistToIndex(utf8);
        }

        private long GetPowerampPlaylistId()
        {
            using (ICursor c = ContentResolver.Query(PowerampAPI.ROOT_URI.BuildUpon().AppendEncodedPath("playlists").Build(), new string[] { "_id", "name" }, null, null, "_id"))
            {
                if (c != null)
                {
                    long id = 0;
                    string name;
                    while (c.MoveToNext())
                    {
                        id = c.GetLong(0);
                        name = c.GetString(1);
                        if (name == "haplaylist.m3u8")
                        {
                            return id;
                        }
                    }
                }
            }
            throw new HaException("Could not find playlist");
        }

        private void startPoweramp()
        {
            try
            {
                Intent intent = new Intent(PowerampAPI.ACTION_API_COMMAND);
                intent.PutExtra(PowerampAPI.COMMAND, PowerampAPI.Commands.OPEN_TO_PLAY)
                        .SetData(PowerampAPI.ROOT_URI.BuildUpon()
                        .AppendEncodedPath("playlists")
                        .AppendEncodedPath(GetPowerampPlaylistId().ToString())
                        .AppendEncodedPath("files")
                        .Build());
                Intent explicit_intent = CreateExplicitFromImplicitIntent(this, intent);
                StartService(explicit_intent);
            }
            catch (HaException e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
        }

        private void startViewIntent()
        {
            Intent intent = new Intent(Intent.ActionView);
            string mime = MimeTypeMap.Singleton.GetMimeTypeFromExtension("m3u");
            intent.SetDataAndType(uriFromPath(getPlaylistPath(false)), mime);
            if (intent.ResolveActivity(PackageManager) != null)
            {
                StartActivity(intent);
            }
        }

        private static Intent CreateExplicitFromImplicitIntent(Context context, Intent implicitIntent)
        {
            // Retrieve all services that can match the given intent
            PackageManager pm = context.PackageManager;
            IList<ResolveInfo> resolveInfo = pm.QueryIntentServices(implicitIntent, 0);

            // Make sure only one match was found
            if (resolveInfo == null || resolveInfo.Count != 1)
            {
                return null;
            }

            // Get component info and create ComponentName
            ResolveInfo serviceInfo = resolveInfo[0];
            string packageName = serviceInfo.ServiceInfo.PackageName;
            string className = serviceInfo.ServiceInfo.Name;
            ComponentName component = new ComponentName(packageName, className);

            // Create a new intent. Use the old one for extras and such reuse
            Intent explicitIntent = new Intent(implicitIntent);

            // Set the component to be explicit
            explicitIntent.SetComponent(component);

            return explicitIntent;
        }

        [TargetApi(Value = 23)]
        private void RequestPermissions()
        {
            string[] perms = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
            if (perms.Any(x => CheckSelfPermission(x) != Permission.Granted))
            {
                RequestPermissions(perms, 1337);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == 1337)
            {
                for (int i = 0; i < grantResults.Length; i++)
                {
                    if (grantResults[i] != Permission.Granted)
                    {
                        new AlertDialog.Builder(this)
                            .SetTitle("Error")
                            .SetMessage("HaPlaylist must have access to storage in order to function.")
                            .SetPositiveButton("Exit", (s, e) =>
                            {
                                Finish();
                            })
                            .Show();
                        return;
                    }
                }
            }
        }
    }
}

