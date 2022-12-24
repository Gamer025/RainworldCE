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
            //AddFood has protection against going over max but not for negatives
            if (result < 0 && (EventHelpers.MainPlayer.realizedCreature as Player).FoodInStomach == 0) return;
            (EventHelpers.MainPlayer.realizedCreature as Player).AddFood(result);
            if (result < 0)
            {
                foreach (RoomCamera camera in game.cameras)
                {
                    camera.hud.foodMeter.circles[(EventHelpers.MainPlayer.realizedCreature as Player).FoodInStomach].EatFade();
                    camera.hud.foodMeter.showCount--;
                }
            }
        }
    }
}
