using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Fills up players foodmeter but makes them slightly slower and heavier till next cycle
    /// </summary>
    internal class Thanksgiving : CEEvent
    {
        public Thanksgiving()
        {
            _name = "Thanksgiving";
            _description = "Food up, agility down";
            //Doesn't really do anything, but prevents it from rolling multiple times and lets the player know its there
            _activeTime = 9999;
        }

        public override void StartupTrigger()
        {
            //Resets itself when cycle ends
            (EventHelpers.MainPlayer.realizedCreature as Player).AddFood(
                (EventHelpers.MainPlayer.realizedCreature as Player).MaxFoodInStomach - (EventHelpers.MainPlayer.realizedCreature as Player).FoodInStomach);
            game.session.characterStats.bodyWeightFac += 0.20f;
            game.session.characterStats.runspeedFac -= 0.20f;
        }
    }
}
