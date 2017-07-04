/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using HaPlaylist.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HaPlaylist
{
    public partial class MainPage : ContentPage
	{
        private Data data;

        public MainPage() : this(null)
        {
        }

		public MainPage(Data data)
		{
			InitializeComponent();
            this.data = data;
            BindingContext = data;
		}

        IEnumerable<Song> SearchSongsInternal(SongLibrary library, string query)
        {
            var stream = CharStreams.fromstring(query);
            var lexer = new HaGrammarLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new HaGrammarParser(tokens) { ErrorHandler = new BailErrorStrategy() };
            var evaluator = new QueryEvaluator(library);
            try
            {
                return evaluator.Visit(parser.query());
            }
            catch (ParseCanceledException e)
            {
                throw e.InnerException;
            }
        }

        async Task<IEnumerable<string>> SearchSongs(SongLibrary library, string query)
        {
            data.IsLoading = true;
            try
            {
                return SearchSongsInternal(library, query).Select(x => x.Path);
            }
            catch (InputMismatchException)
            {
                await DisplayAlert("Error", "Bad input!", "OK");
            }
            catch (NoViableAltException)
            {
                await DisplayAlert("Error", "Other bad input!", "OK");
            }
            finally
            {
                data.IsLoading = false;
            }
            return null;
        }

        private async void PowerAMP_Clicked(object sender, EventArgs e)
        {
            var playlist = await SearchSongs(data.MediaProvider.LoadLibrary(), data.Query);
            if (playlist != null)
                data.MediaProvider.LaunchPowerAMP(playlist);
        }

        private async void Other_Clicked(object sender, EventArgs e)
        {
            var playlist = await SearchSongs(data.MediaProvider.LoadLibrary(), data.Query);
            if (playlist != null)
                data.MediaProvider.LaunchMediaPlayer(playlist);
        }

        private async void Favorites_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FavoritesPage(data));
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SavePage(data));
        }
    }
}
