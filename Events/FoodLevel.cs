using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// 50% chance to remove or add a food pip to the player, triggers 3 times
    /// </summary>
    internal class FoodLevel : CEEvent
    {
        public FoodLevel()
        {
            _name = "Digestive reordering";
            _description = "Nobody truely knows how Slugcats digest food internally";
            _activeTime = 10;
            _repeatEverySec = 3;
            _allowMultiple = true;
        }

        public override void RecurringTrigger()
        {
            //Add or remove 1 Food pip 3 times
            int[] possibleValues = new int[2] { -1, 1 };
            int result = possibleValues[rnd.Next(possibleValues.Length)];

            if (result > 0)
            {
                (EventHelpers.MainPlayer.realizedCreature as Player).AddFood(result);
            }
            else if (result < 0)
            {
                (EventHelpers.MainPlayer.realizedCreature as Player).SubtractFood(Math.Abs(result));
            }
        }
    }
}
