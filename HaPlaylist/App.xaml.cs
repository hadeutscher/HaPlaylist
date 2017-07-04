using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace HaPlaylist
{
	public partial class App : Application
	{
		public App (Data data)
		{
			InitializeComponent();

			MainPage = new HaPlaylist.MainPage(data);
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
