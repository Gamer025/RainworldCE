using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    internal class Hunger : CEEvent
    {
        /// <summary>
        /// Removes 1 food pip on start and after that every minute until cycle end
        /// </summary>
        public Hunger()
        {
            _name = "Unending Hunger";
            _description = "A never ending strive for more";
            //Will only end at the end of the cycle
            _activeTime = 9999;
            _repeatEverySec = 60;
            _allowMultiple = true;
        }

        public override void StartupTrigger()
        {
            RecurringTrigger();
        }
        public override void RecurringTrigger()
        {
            if ((EventHelpers.MainPlayer.realizedCreature as Player).FoodInStomach == 0) return;
            (EventHelpers.MainPlayer.realizedCreature as Player).AddFood(-1);
            foreach (RoomCamera camera in game.cameras)
            {
                camera.hud.foodMeter.circles[(EventHelpers.MainPlayer.realizedCreature as Player).FoodInStomach].EatFade();
                camera.hud.foodMeter.showCount--;
            }
        }
    }
}
