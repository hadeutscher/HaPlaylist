/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using Xamarin.Forms;

namespace HaPlaylist
{
    public partial class App : Application
	{
        public App() : this(null)
        {
        }

        public App (Data data)
		{
			InitializeComponent();

            MainPage = new NavigationPage(new HaPlaylist.MainPage(data));
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
