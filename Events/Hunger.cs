﻿namespace RainWorldCE.Events
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
            (EventHelpers.MainPlayer.realizedCreature as Player).SubtractFood(1);
        }
    }
}
