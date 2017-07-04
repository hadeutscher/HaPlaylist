/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Annotation;
using Android;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using Android.Content;
using Android.Webkit;
using Android.Database;
using System.IO;
using Android.Provider;
using HaPlaylist.Grammar;

namespace HaPlaylist.Droid
{
    [Activity(Label = "HaPlaylist", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IMediaProvider
    {
        #region Permissions
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
        #endregion

        #region Templates
        private List<Template> LoadTemplates()
        {
            using (TemplatesDB db = new TemplatesDB(this))
            {
                return db.GetData();
            }
        }

        private void AddTemplate(Template template)
        {
            using (TemplatesDB db = new TemplatesDB(this))
            {
                db.AddData(template);
            }
        }

        private void RemoveTemplate(Template template)
        {
            using (TemplatesDB db = new TemplatesDB(this))
            {
                db.RemoveData(template.Name);
            }
        }

        private void Templates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Template template in e.NewItems.Cast<Template>())
                {
                    AddTemplate(template);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Template template in e.OldItems.Cast<Template>())
                {
                    RemoveTemplate(template);
                }
            }
        }
        #endregion

        #region Android Playlist/Intents Hacking
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
            cv.Put("name", name);
            cv.Put("_data", getPlaylistPath(utf8));
            cv.Put("date_added", DateTime.Now.ToFileTime());
            cv.Put("date_modified", DateTime.Now.ToFileTime());
            ContentResolver.Insert(MediaStore.Audio.Playlists.ExternalContentUri, cv);
        }

        private void CreatePlaylist(IEnumerable<string> playlist, bool utf8)
        {
            string playlist_path = getPlaylistPath(utf8);
            using (StreamWriter sw = File.CreateText(playlist_path))
            {
                foreach (string file in playlist)
                {
                    sw.WriteLine(file);
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
            throw new HaTagLib.HaException("Could not find playlist");
        }

        private void StartPoweramp()
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
            catch (HaTagLib.HaException e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }
        }

        private void StartViewIntent()
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
        #endregion

        public SongLibrary LoadLibrary()
        {
            var songs = new List<Song>();
            foreach (string file in HaTagLib.Utils.ExpandDirectories(new string[] { getMusicFolderPath() }))
            {
                try
                {
                    using (HaTagLib.IHaTagger ht = HaTagLib.TaggerFactory.CreateTagger(file, false))
                    {
                        ht.ValueTags.Add("path", file.ToLower());
                        songs.Add(new Song(file, new EvaluationContext(new HashSet<string>(ht.Tags), ht.ValueTags, null)));
                    }
                }
                catch (HaTagLib.HaException) { }
            }
            return new SongLibrary(songs);
        }

        public void LaunchPowerAMP(IEnumerable<string> playlist)
        {
            CreatePlaylist(playlist, true);
            StartPoweramp();
        }

        public void LaunchMediaPlayer(IEnumerable<string> playlist)
        {
            CreatePlaylist(playlist, false);
            StartViewIntent();
        }

        private Data Initialize()
        {
            RequestPermissions();
            var result = new Data() { MediaProvider = this };
            LoadTemplates().ForEach(x => result.Templates.Add(x));
            result.Templates.CollectionChanged += Templates_CollectionChanged;
            return result;
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new HaPlaylist.App(Initialize()));
        }
    }
}