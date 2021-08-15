using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс проигрывателя музыки
    internal enum PlayMusicMode { None, MainTheme, InGameMusic };

    internal sealed class PlayerMusic
    {
        private readonly MediaPlayer mpMusic;
        private readonly MediaPlayer mpMainTheme;
        private readonly List<string> playlistCurrent = new List<string>();
        private readonly List<string> playlistFull = new List<string>();
        private readonly Random rnd = new Random();

        public PlayerMusic(string dirResources)
        {
            mpMusic = new System.Windows.Media.MediaPlayer();
            mpMainTheme = new System.Windows.Media.MediaPlayer();
            mpMainTheme.Open(new Uri(dirResources + @"Music\Themes\main_menu_music.mp3"));

            string[] files = Directory.GetFiles(dirResources + @"Music\Music");
            playlistFull.AddRange(files);

            Debug.Assert(playlistFull.Count > 0);
        }

        internal bool EnableMusic { get; set; } = true;
        internal PlayMusicMode Mode { get; private set; } = PlayMusicMode.None;

        internal void PlayMainTheme()
        {
            Debug.Assert((Mode == PlayMusicMode.InGameMusic) || (Mode == PlayMusicMode.None));

            Mode = PlayMusicMode.MainTheme;
            if (EnableMusic)
            {
                mpMusic.Stop();
                mpMainTheme.Play();
            }
        }

        internal void PlayMusic()
        {
            Debug.Assert(Mode == PlayMusicMode.MainTheme);

            Mode = PlayMusicMode.InGameMusic;
            if (EnableMusic)
            {
                mpMainTheme.Stop();
                PlayNextMusic();
            }
        }

        internal void PlayWinLobbyTheme()
        {

        }

        internal void PlayLoseLobbyTheme()
        {

        }

        internal void StopPlay()
        {
            mpMainTheme.Stop();
        }

        internal void RefreshPlaylist()
        {
            playlistCurrent.Clear();
            playlistCurrent.AddRange(playlistFull);
            Debug.Assert(playlistCurrent.Count > 0);
        }

        internal void PlayNextMusic()
        {
            if (playlistCurrent.Count == 0)
                RefreshPlaylist();

            int i = rnd.Next(playlistCurrent.Count);
            mpMusic.Open(new Uri(playlistCurrent[i]));
            mpMusic.Play();
            playlistCurrent.RemoveAt(i);
        }
    }
}
