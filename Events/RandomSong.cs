using Expedition;
using Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainWorldCE.Events
{
    internal class RandomSong : CEEvent
    {
        public RandomSong()
        {
            _name = "Jukebox Hero";
            _description = "I hope you like this one";
            _activeTime = 1;
            _repeatEverySec = 1;
        }

        public override void StartupTrigger()
        {
            if (game.manager.musicPlayer != null)
            {
                game.manager.musicPlayer.FadeOutAllSongs(1f);
            }
            else
            {
                game.manager.musicPlayer = new MusicPlayer(game.manager);
                game.manager.sideProcesses.Add(game.manager.musicPlayer);
                game.manager.musicPlayer.UpdateMusicContext(game.manager.currentMainLoop);
            }

            
        }

        public override void RecurringTrigger()
        {
            List<string> allSongs = ExpeditionProgression.GetUnlockedSongs().Values.ToList();

            game.manager.musicPlayer.GameRequestsSong(new MusicEvent()
            {
                songName = allSongs[rnd.Next(allSongs.Count)],
                cyclesRest = 0,
                prio = 0.5f
            });
        }
    }
}
