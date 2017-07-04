/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using Android.Content;
using Android.Net;

namespace HaPlaylist
{
    public sealed class PowerampAPI
    {
        /**
         * Defines PowerampAPI version, which could be also 200 and 210 for older Poweramps.
         */
        public static readonly int VERSION = 533;

        /**
         * No id flag.
         */
        public static readonly int NO_ID = 0;

        public static readonly string AUTHORITY = "com.maxmpz.audioplayer.data";

        public static readonly Uri ROOT_URI = new Uri.Builder().Scheme("content").Authority(AUTHORITY).Build();

        /**
         * Uri query parameter - filter.
         */
        public static readonly string PARAM_FILTER = "flt";
        /**
         * Uri query parameter - shuffle mode.
         */
        public static readonly string PARAM_SHUFFLE = "shf";


        /**
         * Poweramp Control action.
         * Should be sent with sendBroadcast().
         * Extras:
         * 	- cmd - int - command to execute.
         */
        public static readonly string ACTION_API_COMMAND = "com.maxmpz.audioplayer.API_COMMAND";

        public static Intent newAPIIntent()
        {
            return new Intent(ACTION_API_COMMAND).SetComponent(PLAYER_SERVICE_COMPONENT_NAME);
        }

        /**
         * ACTION_API_COMMAND extra.
         * Int.
         */
        public static readonly string COMMAND = "cmd";


        /** 
         * 
         * Commonm extras:
         * - beep - boolean - (optional) if true, Poweramp will beep on playback command
         */
        public static class Commands
        {
            /**
             * Extras:
             * - keepService - boolean - (optional) if true, Poweramp won't unload player service. Notification will be appropriately updated.
             */
            public static readonly int TOGGLE_PLAY_PAUSE = 1;
            /**
             * Extras:
             * - keepService - boolean - (optional) if true, Poweramp won't unload player service. Notification will be appropriately updated.
             */
            public static readonly int PAUSE = 2;
            public static readonly int RESUME = 3;
            /**
             * NOTE: subject to 200ms throttling.
             */
            public static readonly int NEXT = 4;
            /**
             * NOTE: subject to 200ms throttling.
             */
            public static readonly int PREVIOUS = 5;
            /**
             * NOTE: subject to 200ms throttling.
             */
            public static readonly int NEXT_IN_CAT = 6;
            /**
             * NOTE: subject to 200ms throttling.
             */
            public static readonly int PREVIOUS_IN_CAT = 7;
            /**
             * Extras:
             * - showToast - boolean - (optional) if false, no toast will be shown. Applied for cycle only.
             * - repeat - int - (optional) if exists, appropriate mode will be directly selected, otherwise modes will be cycled, see Repeat class.
             */
            public static readonly int REPEAT = 8;
            /**
             * Extras:
             * - showToast - boolean - (optional) if false, no toast will be shown. Applied for cycle only.
             * - shuffle - int - (optional) if exists, appropriate mode will be directly selected, otherwise modes will be cycled, see Shuffle class.
             */
            public static readonly int SHUFFLE = 9;
            public static readonly int BEGIN_FAST_FORWARD = 10;
            public static readonly int END_FAST_FORWARD = 11;
            public static readonly int BEGIN_REWIND = 12;
            public static readonly int END_REWIND = 13;
            public static readonly int STOP = 14;
            /**
             * Extras:
             * - pos - int - seek position in seconds.
             */
            public static readonly int SEEK = 15;
            public static readonly int POS_SYNC = 16;

            /**
             * Data:
             * - uri, following URIs are recognized:
             * 	- file://path
             * 	- content://com.maxmpz.audioplayer.data/... (see below)
             * 
             * # means some numeric id (track id for queries ending with /files, otherwise - appropriate category id). 
             * If song id (in place of #) is not specified, Poweramp plays whole list starting from the specified song,
             * or from first one, or from random one in shuffle mode.
             * 
             * All queries support following params (added as URL encoded params, e.g. content://com.maxmpz.audioplayer.data/files?lim=10&flt=foo):
             * lim - integer - SQL LIMIT, which limits number of rows returned
             * flt - string - filter substring. Poweramp will return only matching rows (the same way as returned in Poweramp lists UI when filter is used).
             * hier - long - hierarchy folder id. Used only to play in shuffle lists/shuffle songs mode while in hierarchy folders view. This is the target folder id
             *               which will be shuffled with the all subfolders in it as one list.
             * shf - integer - shuffle mode (see ShuffleMode class)
             * ssid - long - shuffle session id (for internal use)
             * 
             * Each /files/meta subquery returns special crafted query with some metainformation provided (it differs in each category, you can explore it by analizing the cols returned).                
            
            - All Songs:
            content://com.maxmpz.audioplayer.data/files
            content://com.maxmpz.audioplayer.data/files/meta
            content://com.maxmpz.audioplayer.data/files/#
            - Most Played
            content://com.maxmpz.audioplayer.data/most_played
            content://com.maxmpz.audioplayer.data/most_played/files
            content://com.maxmpz.audioplayer.data/most_played/files/meta
            content://com.maxmpz.audioplayer.data/most_played/files/#
            
            - Top Rated
            content://com.maxmpz.audioplayer.data/top_rated
            content://com.maxmpz.audioplayer.data/top_rated/files
            content://com.maxmpz.audioplayer.data/top_rated/files/meta
            content://com.maxmpz.audioplayer.data/top_rated/files/#
            - Recently Added
            content://com.maxmpz.audioplayer.data/recently_added
            content://com.maxmpz.audioplayer.data/recently_added/files
            content://com.maxmpz.audioplayer.data/recently_added/files/meta
            content://com.maxmpz.audioplayer.data/recently_added/files/#
            
            - Recently Played
            content://com.maxmpz.audioplayer.data/recently_played
            content://com.maxmpz.audioplayer.data/recently_played/files
            content://com.maxmpz.audioplayer.data/recently_played/files/meta
            content://com.maxmpz.audioplayer.data/recently_played/files/#
            
            - Plain folders view (just files in plain folders list)
            content://com.maxmpz.audioplayer.data/folders
            content://com.maxmpz.audioplayer.data/folders/#
            content://com.maxmpz.audioplayer.data/folders/#/files
            content://com.maxmpz.audioplayer.data/folders/#/files/meta
            content://com.maxmpz.audioplayer.data/folders/#/files/#
            
            - Hierarchy folders view (files and folders intermixed in one cursor)
            content://com.maxmpz.audioplayer.data/folders/#/folders_and_files
            content://com.maxmpz.audioplayer.data/folders/#/folders_and_files/meta
            content://com.maxmpz.audioplayer.data/folders/#/folders_and_files/#
            content://com.maxmpz.audioplayer.data/folders/files // All folder files, sorted as folders_files sort (for mass ops).
            
            - Genres
            content://com.maxmpz.audioplayer.data/genres
            content://com.maxmpz.audioplayer.data/genres/#/files
            content://com.maxmpz.audioplayer.data/genres/#/files/meta
            content://com.maxmpz.audioplayer.data/genres/#/files/#
            content://com.maxmpz.audioplayer.data/genres/files
            - Artists
            content://com.maxmpz.audioplayer.data/artists
            content://com.maxmpz.audioplayer.data/artists/#
            content://com.maxmpz.audioplayer.data/artists/#/files
            content://com.maxmpz.audioplayer.data/artists/#/files/meta
            content://com.maxmpz.audioplayer.data/artists/#/files/#
            content://com.maxmpz.audioplayer.data/artists/files
            
            - Composers
            content://com.maxmpz.audioplayer.data/composers
            content://com.maxmpz.audioplayer.data/composers/#
            content://com.maxmpz.audioplayer.data/composers/#/files
            content://com.maxmpz.audioplayer.data/composers/#/files/#
            content://com.maxmpz.audioplayer.data/composers/#/files/meta
            content://com.maxmpz.audioplayer.data/composers/files
            
            - Albums 
            content://com.maxmpz.audioplayer.data/albums
            content://com.maxmpz.audioplayer.data/albums/#/files
            content://com.maxmpz.audioplayer.data/albums/#/files/#
            content://com.maxmpz.audioplayer.data/albums/#/files/meta
            content://com.maxmpz.audioplayer.data/albums/files
            
            - Albums by Genres
            content://com.maxmpz.audioplayer.data/genres/#/albums
            content://com.maxmpz.audioplayer.data/genres/#/albums/meta
            content://com.maxmpz.audioplayer.data/genres/#/albums/#/files
            content://com.maxmpz.audioplayer.data/genres/#/albums/#/files/#
            content://com.maxmpz.audioplayer.data/genres/#/albums/#/files/meta
            content://com.maxmpz.audioplayer.data/genres/#/albums/files
            content://com.maxmpz.audioplayer.data/genres/albums
            - Albums by Artists
            content://com.maxmpz.audioplayer.data/artists/#/albums
            content://com.maxmpz.audioplayer.data/artists/#/albums/meta
            content://com.maxmpz.audioplayer.data/artists/#/albums/#/files
            content://com.maxmpz.audioplayer.data/artists/#/albums/#/files/#
            content://com.maxmpz.audioplayer.data/artists/#/albums/#/files/meta
            content://com.maxmpz.audioplayer.data/artists/#/albums/files
            content://com.maxmpz.audioplayer.data/artists/albums
            - Albums by Composers
            content://com.maxmpz.audioplayer.data/composers/#/albums
            content://com.maxmpz.audioplayer.data/composers/#/albums/meta
            content://com.maxmpz.audioplayer.data/composers/#/albums/#/files
            content://com.maxmpz.audioplayer.data/composers/#/albums/#/files/#
            content://com.maxmpz.audioplayer.data/composers/#/albums/#/files/meta
            content://com.maxmpz.audioplayer.data/composers/#/albums/files
            content://com.maxmpz.audioplayer.data/composers/albums
            - Artists Albums
            content://com.maxmpz.audioplayer.data/artists_albums
            content://com.maxmpz.audioplayer.data/artists_albums/meta
            content://com.maxmpz.audioplayer.data/artists_albums/#/files
            content://com.maxmpz.audioplayer.data/artists_albums/#/files/#
            content://com.maxmpz.audioplayer.data/artists_albums/#/files/meta
            content://com.maxmpz.audioplayer.data/artists_albums/files
            
            - Playlists
            content://com.maxmpz.audioplayer.data/playlists
            content://com.maxmpz.audioplayer.data/playlists/#
            content://com.maxmpz.audioplayer.data/playlists/#/files
            content://com.maxmpz.audioplayer.data/playlists/#/files/#
            content://com.maxmpz.audioplayer.data/playlists/#/files/meta
            content://com.maxmpz.audioplayer.data/playlists/files
            
            - Library Search
            content://com.maxmpz.audioplayer.data/search
            
            - Equalizer Presets
            content://com.maxmpz.audioplayer.data/eq_presets
            content://com.maxmpz.audioplayer.data/eq_presets/#
            content://com.maxmpz.audioplayer.data/eq_presets_songs
            content://com.maxmpz.audioplayer.data/queue
            content://com.maxmpz.audioplayer.data/queue/#
             *  
             * Extras:
             * - paused - boolean - (optional) default false. OPEN_TO_PLAY command starts playing the file immediately, unless "paused" extra is true.
             *                       (see PowerampAPI.PAUSED)
             * 
             * - pos - int - (optional) seek to this position in song before playing (see PowerampAPI.Track.POSITION)
             */
            public static readonly int OPEN_TO_PLAY = 20;

            /**
             * Extras:
             * - id - long - preset ID
             */
            public static readonly int SET_EQU_PRESET = 50;

            /**
             * Extras:
             * - value - string - equalizer values, see ACTION_EQU_CHANGED description.
             */
            public static readonly int SET_EQU_string = 51;

            /**
             * Extras:
             * - name - string - equalizer band (bass/treble/preamp/31/62../8K/16K) name
             * - value - float - equalizer band value (bass/treble/, 31/62../8K/16K => -1.0...1.0, preamp => 0..2.0)
             */
            public static readonly int SET_EQU_BAND = 52;

            /**
             * Extras:
             * - equ - boolean - if exists and true, equalizer is enabled
             * - tone - boolean - if exists and true, tone is enabled
             */
            public static readonly int SET_EQU_ENABLED = 53;

            /**
             * Used by Notification controls to stop pending/paused service/playback and unload/remove notification.
             * Since 2.0.6
             */
            public static readonly int STOP_SERVICE = 100;
        }

        /**
         * Extra.
         * Mixed.
         */
        public static readonly string API_VERSION = "api";

        /**
         * Extra.
         * Mixed.
         */
        public static readonly string CONTENT = "content";

        /**
         * Extra.
         * string.
         */
        public static readonly string PACKAGE = "pak";

        /**
         * Extra.
         * string.
         */
        public static readonly string LABEL = "label";

        /**
         * Extra.
         * Boolean.
         */
        public static readonly string AUTO_HIDE = "autoHide";

        /**
         * Extra.
         * Bitmap.
         */
        public static readonly string ICON = "icon";

        /**
         * Extra.
         * Boolean.
         */
        public static readonly string MATCH_FILE = "matchFile";

        /**
         * Extra.
         * Boolean
         */
        public static readonly string SHOW_TOAST = "showToast";

        /**
         * Extra.
         * string.
         */
        public static readonly string NAME = "name";

        /**
         * Extra.
         * Mixed.
         */
        public static readonly string VALUE = "value";

        /**
         * Extra.
         * Boolean.
         */
        public static readonly string EQU = "equ";

        /**
         * Extra.
         * Boolean.
         */
        public static readonly string TONE = "tone";

        /**
         * Extra.
         * Boolean.
         * Since 2.0.6
         */
        public static readonly string KEEP_SERVICE = "keepService";

        /**
         * Extra.
         * Boolean
         * Since build 533
         */
        public static readonly string BEEP = "beep";


        /**
         * Poweramp track changed.
         * Sticky intent.
         * Extras:
         * - track - bundle - Track bundle, see Track class.
         * - ts - long - timestamp of the event (System.currentTimeMillis()).
         *  Note, that by default Poweramp won't search/download album art when screen is OFF, but will do that on next screen ON event.  
         */
        public static readonly string ACTION_TRACK_CHANGED = "com.maxmpz.audioplayer.TRACK_CHANGED";

        /**
         * Album art was changed. Album art can be the same for whole album/folder, thus usually it will be updated less frequently comparing to TRACK_CHANGE.
         * If both aaPath and aaBitmap extras are missing that means no album art exists for the current track(s).
         * Note that there is no direct Album Art to track relation, i.e. both track and album art can change independently from each other -
         * for example - when new album art asynchronously downloaded from internet or selected by user.
         * Sticky intent.
         * Extras:
         * - aaPath - string - (optional) if exists, direct path to the cached album art is available.
         * - aaBitmap - Bitmap - (optional)	if exists, some rescaled up to 500x500 px album art bitmap is available.
         *              There will be aaBitmap if aaPath is available, but image is bigger than 600x600 px.
         * - delayed - boolean - (optional) if true, this album art was downloaded or selected later by user.
                  
         * - ts - long - timestamp of the event (System.currentTimeMillis()).
         */
        public static readonly string ACTION_AA_CHANGED = "com.maxmpz.audioplayer.AA_CHANGED";

        /**
         * Poweramp playing status changed (track started/paused/resumed/ended, playing ended).
         * Sticky intent.
         * Extras: 
         * - status - string - one of the STATUS_* values
         * - pos - int - (optional) current in-track position in seconds.
         * - ts - long - timestamp of the event (System.currentTimeMillis()).
         * - additional extras - depending on STATUS_ value (see STATUS_* description below).
         */
        public static readonly string ACTION_STATUS_CHANGED = "com.maxmpz.audioplayer.STATUS_CHANGED";

        /**
         * NON sticky intent.
         * - pos - int - current in-track position in seconds. 
         */
        public static readonly string ACTION_TRACK_POS_SYNC = "com.maxmpz.audioplayer.TPOS_SYNC";

        /**
         * Poweramp repeat or shuffle mode changed.
         * Sticky intent.
         * Extras:
         * - repeat - int - new repeat mode. See RepeatMode class.
         * - shuffle - int - new shuffle mode. See ShuffleMode class.
         * - ts - long - timestamp of the event (System.currentTimeMillis()).	 * 
         */
        public static readonly string ACTION_PLAYING_MODE_CHANGED = "com.maxmpz.audioplayer.PLAYING_MODE_CHANGED";

        /**
         * Poweramp equalizer settings changed.
         * Sticky intent.
         * Extras:
         * - name - string - preset name. If no name extra exists, it's not a preset.
         * - id - long - preset id. If no id extra exists, it's not a preset.
         * - value - string - equalizer and tone values in format:
         *   	bass=pos_float|treble=pos_float|31=float|62=float|....|16K=float|preamp=0.0 ... 2.0
         *      where float = -1.0 ... 1.0, pos_float = 0.0 ... 1.0
         * - equ - boolean - true if equalizer bands are enabled
         * - tone - boolean - truel if tone bands are enabled
         * - ts - long - timestamp of the event (System.currentTimeMillis()).
         */
        public static readonly string ACTION_EQU_CHANGED = "com.maxmpz.audioplayer.EQU_CHANGED";

        /**
         * Special actions for com.maxmpz.audioplayer.PlayerUIActivity only.
         */
        public static readonly string ACTION_SHOW_CURRENT = "com.maxmpz.audioplayer.ACTION_SHOW_CURRENT";
        public static readonly string ACTION_SHOW_LIST = "com.maxmpz.audioplayer.ACTION_SHOW_LIST";


        public static readonly string PACKAGE_NAME = "com.maxmpz.audioplayer";
        public static readonly string PLAYER_SERVICE_NAME = "com.maxmpz.audioplayer.player.PlayerService";

        public static readonly ComponentName PLAYER_SERVICE_COMPONENT_NAME = new ComponentName(PACKAGE_NAME, PLAYER_SERVICE_NAME);

        public static readonly string ACTIVITY_PLAYER_UI = "com.maxmpz.audioplayer.PlayerUIActivity";
        public static readonly string ACTIVITY_EQ = "com.maxmpz.audioplayer.EqActivity";

        /**
         * If com.maxmpz.audioplayer.ACTION_SHOW_LIST action is sent to this activity, it will react to some extras.
         * Extras:
         * Data:
         * - uri - uri of the list to display. 
         */
        public static readonly string ACTIVITY_PLAYLIST = "com.maxmpz.audioplayer.PlayListActivity";
        public static readonly string ACTIVITY_SETTINGS = "com.maxmpz.audioplayer.preference.SettingsActivity";

        /**
         * Extra.
         * string.
         */
        public static readonly string ALBUM_ART_PATH = "aaPath";

        /**
         * Extra.
         * Bitmap. 
         */
        public static readonly string ALBUM_ART_BITMAP = "aaBitmap";

        /**
         * Extra.
         * boolean. 
         */
        public static readonly string DELAYED = "delayed";


        /**
         * Extra.
         * long.
         */
        public static readonly string TIMESTAMP = "ts";

        /**
         * STATUS_CHANGED extra. See Status class for values.
         * Int.
         */
        public static readonly string STATUS = "status";

        /**
         * STATUS extra values.
         */
        public static class Status
        {
            /**
             * STATUS_CHANGED status value - track has been started to play or has been paused.
             * Note that Poweramp will start track immediately into this state when it's just loaded to avoid STARTED => PAUSED transition. 
             * Additional extras:
             * 	track - bundle - track info 
             * 	paused - boolean - true if track paused, false if track resumed
             */
            public static readonly int TRACK_PLAYING = 1;

            /**
             * STATUS_CHANGED status value - track has been ended. Note, this intent will NOT be sent for just finished song IF Poweramp advances to the next song.
             * Additional extras:
             * 	track - bundle - track info 
             *  failed - boolean - true if track failed to play
             */
            public static readonly int TRACK_ENDED = 2;

            /**
             * STATUS_CHANGED status value - Poweramp finished playing some list and stopped.
             */
            public static readonly int PLAYING_ENDED = 3;
        }


        /**
         * STATUS_CHANGED trackEnded extra. 
         * Boolean. True if track failed to play.
         */
        public static readonly string FAILED = "failed";

        /**
         * STATUS_CHANGED trackStarted/trackPausedResumed extra.
         * Boolean. True if track is paused.
         */
        public static readonly string PAUSED = "paused";

        /**
         * PLAYING_MODE_CHANGED extra. See ShuffleMode class.
         * Integer.
         */
        public static readonly string SHUFFLE = "shuffle";

        /**
         * PLAYING_MODE_CHANGED extra. See RepeatMode class.
         * Integer.
         */
        public static readonly string REPEAT = "repeat";


        /**
         * Extra.
         * Long.
         */
        public static readonly string ID = "id";

        /**
         * STATUS_CHANGED track extra.
         * Bundle.
         */
        public static readonly string TRACK = "track";


        /**
         * shuffle extras values.
         */
        public static class ShuffleMode
        {
            public static readonly int SHUFFLE_NONE = 0;
            public static readonly int SHUFFLE_ALL = 1;
            public static readonly int SHUFFLE_SONGS = 2;
            public static readonly int SHUFFLE_CATS = 3; // Songs in order.
            public static readonly int SHUFFLE_SONGS_AND_CATS = 4; // Songs shuffled.
        }

        /**
         * repeat extras values.
         */
        public static class RepeatMode
        {
            public static readonly int REPEAT_NONE = 0;
            public static readonly int REPEAT_ON = 1;
            public static readonly int REPEAT_ADVANCE = 2;
            public static readonly int REPEAT_SONG = 3;
        }


        /**
         * STATUS_CHANGED track extra fields.
         */
        public static class Track
        {
            /**
             * Id of the current track.
             * Can be a playlist entry id.
             * Long.
             */
            public static readonly string ID = "id";

            /**
             * "Real" id. In case of playlist entry, this is always resolved to Poweramp folder_files table row ID or System Library MediaStorage.Audio._ID. 
             * Long.
             */
            public static readonly string REAL_ID = "realId";

            /**
             * Category type.
             * See Track.Type class.
             * Int.
             */
            public static readonly string TYPE = "type";

            /**
             * Category URI match.
             * Int.
             */
            public static readonly string CAT = "cat";

            /**
             * Boolean.
             */
            public static readonly string IS_CUE = "isCue";

            /**
             * Category URI.
             * Uri.
             */
            public static readonly string CAT_URI = "catUri";

            /**
             * File type. See Track.FileType.
             * Integer.
             */
            public static readonly string FILE_TYPE = "fileType";

            /**
             * Song file path.
             * string
             */
            public static readonly string PATH = "path";

            /**
             * Song title.
             * string
             */
            public static readonly string TITLE = "title";

            /**
             * Song album.
             * string.
             */
            public static readonly string ALBUM = "album";

            /**
             * Song artist.
             * string.
             */
            public static readonly string ARTIST = "artist";

            /**
             * Song duration in seconds.
             * Int.
             */
            public static readonly string DURATION = "dur";

            /**
             * Position in song in seconds.
             * Int.
             */
            public static readonly string POSITION = "pos";

            /**
             * Position in a list.
             * Int.
             */
            public static readonly string POS_IN_LIST = "posInList";

            /**
             * List size.
             * Int.
             */
            public static readonly string LIST_SIZE = "listSize";

            /**
             * Song sample rate.
             * Int.
             */
            public static readonly string SAMPLE_RATE = "sampleRate";

            /**
             * Song number of channels.
             * Int.
             */
            public static readonly string CHANNELS = "channels";

            /**
             * Song average bitrate.
             * Int.
             */
            public static readonly string BITRATE = "bitRate";

            /**
             * Resolved codec name for the song.
             * Int.
             */
            public static readonly string CODEC = "codec";

            /**
             * Track flags.
             * Int.
             */
            public static readonly string FLAGS = "flags";

            /**
             * Track.fileType values.
             */
            public static class FileType
            {
                public static readonly int mp3 = 0;
                public static readonly int flac = 1;
                public static readonly int m4a = 2;
                public static readonly int mp4 = 3;
                public static readonly int ogg = 4;
                public static readonly int wma = 5;
                public static readonly int wav = 6;
                public static readonly int tta = 7;
                public static readonly int ape = 8;
                public static readonly int wv = 9;
                public static readonly int aac = 10;
                public static readonly int mpga = 11;
                public static readonly int amr = 12;
                public static readonly int _3gp = 13;
                public static readonly int mpc = 14;
                public static readonly int aiff = 15;
                public static readonly int aif = 16;
            }

            /**
             * Track.flags bitset values. First 3 bits = FLAG_ADVANCE_* 
             */
            public static class Flags
            {
                public static readonly int FLAG_ADVANCE_NONE = 0;
                public static readonly int FLAG_ADVANCE_FORWARD = 1;
                public static readonly int FLAG_ADVANCE_BACKWARD = 2;
                public static readonly int FLAG_ADVANCE_FORWARD_CAT = 3;
                public static readonly int FLAG_ADVANCE_BACKWARD_CAT = 4;

                public static readonly int FLAG_ADVANCE_MASK = 0x7; // 111

                public static readonly int FLAG_NOTIFICATION_UI = 0x20;
                public static readonly int FLAG_FIRST_IN_PLAYER_SESSION = 0x40; // Currently used just to indicate that track is first in playerservice session.			
            }

            public static class Cats
            {
                public static readonly int ROOT = 0;
                public static readonly int FOLDERS = 10;
                public static readonly int GENRES_ID_ALBUMS = 210;
                public static readonly int ALBUMS = 200;
                public static readonly int GENRES = 320;
                public static readonly int ARTISTS = 500;
                public static readonly int ARTISTS_ID_ALBUMS = 220;
                public static readonly int ARTISTS__ALBUMS = 250;
                public static readonly int COMPOSERS = 600;
                public static readonly int COMPOSERS_ID_ALBUMS = 230;
                public static readonly int PLAYLISTS = 100;
                public static readonly int QUEUE = 800;
                public static readonly int MOST_PLAYED = 43;
                public static readonly int TOP_RATED = 48;
                public static readonly int RECENTLY_ADDED = 53;
                public static readonly int RECENTLY_PLAYED = 58;
            }


            public static class Scanner
            {

                /**
                 * Poweramp Scanner action.
                 * 
                 * Poweramp Scanner scanning process is 2 step:
                 * 1. Folders scan.
                 *    Checks filesystem and updates DB with folders/files structure.
                 * 2. Tags scan.
                 *    Iterates over files in DB with TAG_STATUS == TAG_NOT_SCANNED and scans them with tag scanner.
                 *    
                 * Poweramp Scanner is a IntentService, this means multiple scan requests at the same time (or during another scans) are queued.  
                 * ACTION_SCAN_DIRS actions are prioritized and executed before ACTION_SCAN_TAGS.
                 * 
                 * Poweramp main scan action, which scans either incrementally or from scratch the set of folders, which is configured by user in Poweramp Settings.
                 * Poweramp will always do ACTION_SCAN_TAGS automatically after ACTION_SCAN_DIRS is finished and some changes are required to song tags in DB.
                 * Unless, fullRescan specified, Poweramp will not remove songs if they are missing from filesystem due to unmounted storages.
                 * Normal menu => Rescan calls ACTION_SCAN_DIRS without extras
                 * 
                 * Poweramp Scanner sends appropriate broadcast intents:
                 * ACTION_DIRS_SCAN_STARTED (sticky), ACTION_DIRS_SCAN_FINISHED, ACTION_TAGS_SCAN_STARTED (sticky), ACTION_TAGS_SCAN_PROGRESS, ACTION_TAGS_SCAN_FINISHED, or ACTION_FAST_TAGS_SCAN_FINISHED.
                 * 
                 * Extras:
                 * - fastScan - Poweramp will not check folders and scan files which hasn't been modified from previous scan. Based on files last modified timestamp.
                 *				 Poweramp doesn;t send 
                 *               
                 * - eraseTags - Poweramp will clean all tags from exisiting songs. This causes each song to be re-scanned for tags.
                 *               Warning: as a side effect, cleans CUE tracks from user created playlists. 
                 *               This is because scanner can't incrementaly re-scan CUE sheets, so they are deleted from DB.
                 *               
                 * - fullRescan - Poweramp will also check for folders/files from missing/unmounted storages and will remove them from DB. 
                 *                Warning: removed songs also disappear from user created playlists.
                 *                Used in Poweramp only when user specificaly goes to Settings and does Full Rescan (after e.g. SD card change).                
                 *                                  
                 */
                public static readonly string ACTION_SCAN_DIRS = "com.maxmpz.audioplayer.ACTION_SCAN_DIRS";

                /**
                 * Poweramp Scanner action.
                 * Secondary action, only checks songs with TAG_STATUS set to TAG_NOT_SCANNED. Useful for rescanning just songs (which are already in Poweramp DB) with editied file tag info.  
                 *
                 * Extras:
                 * - fastScan - If true, scanner doesn't send ACTION_TAGS_SCAN_STARTED/ACTION_TAGS_SCAN_PROGRESS/ACTION_TAGS_SCAN_FINISHED intents, 
                 *               just sends ACTION_FAST_TAGS_SCAN_FINISHED when done.
                 *               It doesn't modify scanning logic otherwise.  
                 */
                public static readonly string ACTION_SCAN_TAGS = "com.maxmpz.audioplayer.ACTION_SCAN_TAGS";


                /**
                 * Broadcast.
                 * Poweramp Scanner started folders scan.
                 * This is sticky broadcast, so Poweramp folder scanner running status can be polled via registerReceiver() return value.
                 */
                public static readonly string ACTION_DIRS_SCAN_STARTED = "com.maxmpz.audioplayer.ACTION_DIRS_SCAN_STARTED";
                /**
                 * Broadcast.
                 * Poweramp Scanner finished folders scan.
                 */
                public static readonly string ACTION_DIRS_SCAN_FINISHED = "com.maxmpz.audioplayer.ACTION_DIRS_SCAN_FINISHED";
                /**
                 * Broadcast.
                 * Poweramp Scanner started tag scan.
                 * This is sticky broadcast, so Poweramp tag scanner running status can be polled via registerReceiver() return value.
                 */
                public static readonly string ACTION_TAGS_SCAN_STARTED = "com.maxmpz.audioplayer.ACTION_TAGS_SCAN_STARTED";
                /**
                 * Broadcast.
                 * Poweramp Scanner tag scan in progess.
                 * Extras:
                 * - progress - 0-100 progress of scanning.
                 */
                public static readonly string ACTION_TAGS_SCAN_PROGRESS = "com.maxmpz.audioplayer.ACTION_TAGS_SCAN_PROGRESS";
                /**
                 * Broadcast.
                 * Poweramp Scanner finished tag scan.
                 * Extras:
                 * - track_content_changed - boolean - true if at least on track has been scanned, false if no tags scanned (probably, because all files are up-to-date).
                 */
                public static readonly string ACTION_TAGS_SCAN_FINISHED = "com.maxmpz.audioplayer.ACTION_TAGS_SCAN_FINISHED";
                /**
                 * Broadcast.
                 * Poweramp Scanner finished fast tag scan. Only fired when ACTION_SCAN_TAGS is called with extra fastScan = true.
                 * Extras:
                 * - trackContentChanged - boolean - true if at least on track has been scanned, false if no tags scanned (probably, because all files are up-to-date).
                 */
                public static readonly string ACTION_FAST_TAGS_SCAN_FINISHED = "com.maxmpz.audioplayer.ACTION_FAST_TAGS_SCAN_FINISHED";

                /**
                 * Extra.
                 * Boolean.
                 */
                public static readonly string EXTRA_FAST_SCAN = "fastScan";
                /**
                 * Extra.
                 * Int.
                 */
                public static readonly string EXTRA_PROGRESS = "progress";
                /**
                 * Extra.
                 * Boolean.
                 */
                public static readonly string EXTRA_TRACK_CONTENT_CHANGED = "trackContentChanged";

                /**
                 * Extra.
                 * Boolean.
                 */
                public static readonly string EXTRA_ERASE_TAGS = "eraseTags";

                /**
                 * Extra.
                 * Boolean.
                 */
                public static readonly string EXTRA_FULL_RESCAN = "fullRescan";

                /**
                 * Extra.
                 * string.
                 */
                public static readonly string EXTRA_CAUSE = "cause";
            }

            public static class Settings
            {
                public static readonly string ACTION_EXPORT_SETTINGS = "com.maxmpz.audioplayer.ACTION_EXPORT_SETTINGS";
                public static readonly string ACTION_IMPORT_SETTINGS = "com.maxmpz.audioplayer.ACTION_IMPORT_SETTINGS";

                public static readonly string EXTRA_UI = "ui";
            }
        }
    }
}