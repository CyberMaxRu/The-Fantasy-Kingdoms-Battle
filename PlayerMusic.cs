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
    internal enum PlayMusicMode { None, MainTheme, InGameMusic, EndLobby };

    internal sealed class PlayerMusic
    {
        private readonly MediaPlayer mpMusic;
        private readonly MediaPlayer mpTheme;
        private readonly List<string> playlistCurrent = new List<string>();
        private readonly List<string> playlistFull = new List<string>();
        private readonly Random rnd = new Random();
        private readonly Uri fileMainTheme;
        private readonly Uri fileWinLobbyTheme;
        private readonly Uri fileLossLobbyTheme;

        public PlayerMusic(string dirResources)
        {
            mpMusic = new System.Windows.Media.MediaPlayer();
            mpTheme = new System.Windows.Media.MediaPlayer();

            fileMainTheme = new Uri(dirResources + @"Music\Themes\main_menu_music.mp3");
            fileWinLobbyTheme = new Uri(dirResources + @"Music\Themes\won_music.mp3");
            fileLossLobbyTheme = new Uri(dirResources + @"Music\Themes\loose_music.mp3");

            string[] files = Directory.GetFiles(dirResources + @"Music\Music");
            playlistFull.AddRange(files);

            Debug.Assert(playlistFull.Count > 0);
        }

        internal bool EnableMusic { get; set; } = true;
        internal PlayMusicMode Mode { get; private set; } = PlayMusicMode.None;

        internal void PlayMainTheme()
        {
            Debug.Assert((Mode == PlayMusicMode.InGameMusic) || (Mode == PlayMusicMode.EndLobby) || (Mode == PlayMusicMode.None));

            Mode = PlayMusicMode.MainTheme;
            if (EnableMusic)
            {
                mpMusic.Stop();
                mpTheme.Open(fileMainTheme);
                mpTheme.Play();
            }
        }

        internal void PlayMusic()
        {
            Debug.Assert(Mode == PlayMusicMode.MainTheme);

            Mode = PlayMusicMode.InGameMusic;
            if (EnableMusic)
            {
                mpTheme.Stop();
                PlayNextMusic();
            }
        }

        internal void PlayWinLobbyTheme()
        {
            Mode = PlayMusicMode.EndLobby;

            if (EnableMusic)
            {
                mpMusic.Stop();
                mpTheme.Open(fileWinLobbyTheme);
                mpTheme.Play();
            }
        }

        internal void PlayLossLobbyTheme()
        {
            Mode = PlayMusicMode.EndLobby;

            if (EnableMusic)
            {
                mpMusic.Stop();
                mpTheme.Open(fileLossLobbyTheme);
                mpTheme.Play();
            }
        }

        internal void StopPlay()
        {
            mpTheme.Stop();
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
