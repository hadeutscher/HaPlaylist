using System;
/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HaPlaylist
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FavoritesPage : ContentPage
	{
        private Data data;

        public FavoritesPage() : this(null)
		{
		}

        public FavoritesPage(Data data)
        {
            InitializeComponent();

            this.data = data;
            BindingContext = data;
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            data.Templates.Remove((Template)((MenuItem)sender).CommandParameter);
        }

        private async void Template_Tapped(object sender, EventArgs e)
        {
            string value = ((Template)((TextCell)sender).CommandParameter).Value;
            data.Query = value;
            await Navigation.PopAsync();
        }
    }
}