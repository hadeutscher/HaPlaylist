using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using HaPlaylist.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HaPlaylist
{
	public partial class MainPage : ContentPage
	{
        private Data data;

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

        IEnumerable<string> SearchSongs(SongLibrary library, string query)
        {
            try
            {
                return SearchSongsInternal(library, query).Select(x => x.Path);
            }
            catch (InputMismatchException)
            {
                DisplayAlert("Error", "Bad input!", "OK");
            }
            catch (NoViableAltException)
            {
                DisplayAlert("Error", "Other bad input!", "OK");
            }
            return null;
        }

        private void PowerAMP_Clicked(object sender, EventArgs e)
        {
            var playlist = SearchSongs(data.MediaProvider.LoadLibrary(), data.Query);
            if (playlist != null)
                data.MediaProvider.LaunchPowerAMP(playlist);
        }

        private void Other_Clicked(object sender, EventArgs e)
        {
            var playlist = SearchSongs(data.MediaProvider.LoadLibrary(), data.Query);
            if (playlist != null)
                data.MediaProvider.LaunchMediaPlayer(playlist);
        }
    }
}
