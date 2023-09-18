using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Fast forwards the rain timer dramatically
    /// </summary>
    internal class RainyDay : CEEvent
    {
        public RainyDay()
        {
            _name = "Rainy day";
            _description = "Rain seems to approching faster than usual";
            _activeTime = 30;
            _repeatEverySec = 3;
        }

        public override void RecurringTrigger()
        {
            //Forward time by 7% of the remaining time 10 times
            game.world.rainCycle.timer = (int)Mathf.Lerp(game.world.rainCycle.timer, game.world.rainCycle.cycleLength, 0.07f);
        }
    }
}
