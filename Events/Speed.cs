using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Makes the player faster for some amount of time
    /// </summary>
    internal class Speed : CEEvent
    {
        public Speed()
        {
            _name = "Speeeeed";
            _description = "Gotta go fast";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        public override void StartupTrigger()
        {
            if (ModManager.CoopAvailable)
            {
                foreach (SlugcatStats stats in (game.session as StoryGameSession).characterStatsJollyplayer.Where(x => x is not null))
                {
                    stats.runspeedFac += 1f;
                    stats.poleClimbSpeedFac += 1f;
                    stats.corridorClimbSpeedFac += 1f;
                }
            }
            else
            {
                game.session.characterStats.runspeedFac += 1f;
                game.session.characterStats.poleClimbSpeedFac += 1f;
                game.session.characterStats.corridorClimbSpeedFac += 1f;
            }

        }


        public override void ShutdownTrigger()
        {
            if (ModManager.CoopAvailable)
            {
                foreach (SlugcatStats stats in (game.session as StoryGameSession).characterStatsJollyplayer.Where(x => x is not null))
                {
                    stats.runspeedFac -= 1f;
                    stats.poleClimbSpeedFac -= 1f;
                    stats.corridorClimbSpeedFac -= 1f;
                }
            }
            else
            {
                game.session.characterStats.runspeedFac -= 1f;
                game.session.characterStats.poleClimbSpeedFac -= 1f;
                game.session.characterStats.corridorClimbSpeedFac -= 1f;
            }
        }
    }
}
