/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Collections.ObjectModel;

namespace HaPlaylist
{
    public class Data : PropertyNotifierBase
    {
        private string _query = null;
        public string Query { get => _query ?? (_query = ""); set => SetFieldAndNotify(ref _query, value); }

        private bool _isLoading = false;
        public bool IsLoading { get => _isLoading; set => SetFieldAndNotify(ref _isLoading, value); }

        private ObservableCollection<Template> _templates = null;
        public ObservableCollection<Template> Templates { get => _templates ?? (_templates = new ObservableCollection<Template>()); }

        private IMediaProvider _mediaProvider = null;
        public IMediaProvider MediaProvider { get => _mediaProvider; set => SetFieldAndNotify(ref _mediaProvider, value); }

    }
}
