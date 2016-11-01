﻿/* Copyright (C) 2016 haha01haha01

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

namespace HaPlaylist
{
    [Activity(Label = "HaPlaylist", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string SETTINGS_NAME = "hasettings";
        private const string SPINNER_NAMES = "spinner_names";
        private const string SPINNER_VALUES = "spinner_values";

        private string[] saved_names = new string[0];
        private string[] saved_values = new string[0];

        private static string[] ConvertListToStringArray(ICollection<string> x)
        {
            string[] result = new string[x.Count];
            x.CopyTo(result, 0);
            return result;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            ISharedPreferences prefs = GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private);
            saved_names = ConvertListToStringArray(prefs.GetStringSet(SPINNER_NAMES, new string[0]));
            saved_values = ConvertListToStringArray(prefs.GetStringSet(SPINNER_VALUES, new string[0]));
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, saved_names);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.templateSpinner);
            spinner.Adapter = adapter;
        }


        private void SaveSettings()
        {
            ISharedPreferences prefs = GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private);
            using (ISharedPreferencesEditor editor = prefs.Edit())
            {
                editor.PutStringSet(SPINNER_NAMES, saved_names);
                editor.PutStringSet(SPINNER_VALUES, saved_values);
                editor.Commit();
            }
        }

        private void createPlaylist()
        {
            HaSyntax syntax = new HaSyntax(FindViewById<EditText>(Resource.Id.queryInput).Text);
            string music_folder = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).AbsolutePath;
            string playlist_path = Path.Combine(music_folder, "haplaylist.m3u8");
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
            Intent intent = new Intent(Intent.ActionMediaScannerScanFile);
            intent.SetData(Uri.FromFile(new Java.IO.File(playlist_path)));
            SendBroadcast(intent);
        }

        [Export("powerampButtonClick")]
        public void powerampButtonClick(View v)
        {
            createPlaylist();
            startPoweramp();
        }

        [Export("saveTemplateClick")]
        public void saveTemplateClick(View v)
        {
            // TODO
        }

        private long GetPlaylistId()
        {
            ICursor c = ContentResolver.Query(PowerampAPI.ROOT_URI.BuildUpon().AppendEncodedPath("playlists").Build(), new string[] { "_id", "name" }, null, null, "_id");
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
                        .AppendEncodedPath(GetPlaylistId().ToString())
                        .AppendEncodedPath("files")
                        .Build());
                Intent explicit_intent = CreateExplicitFromImplicitIntent(this, intent);
                StartService(explicit_intent);
            }
            catch (HaException e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long);
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
    }
}

