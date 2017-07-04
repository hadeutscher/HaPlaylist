/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HaPlaylist
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SavePage : ContentPage
	{
        class SaveData : PropertyNotifierBase
        {
            private string _saveName = null;
            public string SaveName { get => _saveName ?? (_saveName = ""); set => SetFieldAndNotify(ref _saveName, value); }

            private Data _parentData = null;
            public Data ParentData { get => _parentData; set => SetFieldAndNotify(ref _parentData, value); }
        }

        private SaveData data;

        public SavePage() : this(null)
        {
        }

		public SavePage(Data data)
        {
			InitializeComponent ();

            this.data = new SaveData() { ParentData = data };
            BindingContext = this.data;
		}

        private async void Save_Clicked(object sender, EventArgs e)
        {
            data.ParentData.Templates.Add(new Template(data.SaveName, data.ParentData.Query));
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            entry.Focus();
        }
    }
}