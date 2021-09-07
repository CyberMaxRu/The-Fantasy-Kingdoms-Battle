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
    internal enum PlayMusicMode { None, MainTheme, InGameMusic, WinLobby, LossLobby };

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

        public PlayerMusic(string dirResources, Settings settings)
        {
            Settings = settings;

            mpMusic = new MediaPlayer();
            mpTheme = new MediaPlayer();

            fileMainTheme = new Uri(dirResources + @"Music\Themes\main_menu_music.mp3");
            fileWinLobbyTheme = new Uri(dirResources + @"Music\Themes\won_music.mp3");
            fileLossLobbyTheme = new Uri(dirResources + @"Music\Themes\loose_music.mp3");

            playlistFull.AddRange(Directory.GetFiles(dirResources + @"Music\Music"));

            Debug.Assert(playlistFull.Count > 0);
        }

        internal Settings Settings { get; }
        internal PlayMusicMode Mode { get; private set; } = PlayMusicMode.None;

        internal void PlayMainTheme()
        {
            //Debug.Assert((Mode == PlayMusicMode.InGameMusic) || (Mode == PlayMusicMode.WinLobby) || (Mode == PlayMusicMode.LossLobby) || (Mode == PlayMusicMode.None));

            Mode = PlayMusicMode.MainTheme;
            if (Settings.PlayMusic)
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
            if (Settings.PlayMusic)
            {
                mpTheme.Stop();
                PlayNextMusic();
            }
        }

        internal void PlayWinLobbyTheme()
        {
            Mode = PlayMusicMode.WinLobby;

            if (Settings.PlayMusic)
            {
                mpMusic.Stop();
                mpTheme.Open(fileWinLobbyTheme);
                mpTheme.Play();
            }
        }

        internal void PlayLossLobbyTheme()
        {
            Mode = PlayMusicMode.LossLobby;

            if (Settings.PlayMusic)
            {
                mpMusic.Stop();
                mpTheme.Open(fileLossLobbyTheme);
                mpTheme.Play();
            }
        }

        internal void StopPlay()
        {
            mpTheme.Stop();
            mpMusic.Stop();
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

        internal void TogglePlayMusic()
        {
            if (Settings.PlayMusic)
            {
                switch (Mode)
                {
                    case PlayMusicMode.MainTheme:
                        PlayMainTheme();
                        break;
                    case PlayMusicMode.WinLobby:
                        PlayWinLobbyTheme();
                        break;
                    case PlayMusicMode.LossLobby:
                        PlayLossLobbyTheme();
                        break;
                    case PlayMusicMode.InGameMusic:
                        PlayNextMusic();
                        break;
                    default:
                        throw new Exception($"Неизвестный режим: {Mode}.");
                }
            }
            else
            {
                StopPlay();
            }
        }
    }
}
