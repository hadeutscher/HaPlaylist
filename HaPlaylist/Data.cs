using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HaPlaylist
{
    public class Data : PropertyNotifierBase
    {
        private string _query = null;
        public string Query { get => _query ?? (_query = ""); set => SetFieldAndNotify(ref _query, value); }

        private ObservableCollection<Template> _templates = null;
        public ObservableCollection<Template> Templates { get => _templates ?? (_templates = new ObservableCollection<Template>()); }

        private IMediaProvider _mediaProvider = null;
        public IMediaProvider MediaProvider { get => _mediaProvider; set => SetFieldAndNotify(ref _mediaProvider, value); }

    }
}
