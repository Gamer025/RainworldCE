using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Sets the rain timer to some random time between 1 minute and max
    /// Also gives them a accurate description of when rain will happen
    /// </summary>
    internal class WeatherForecast : CEEvent
    {
        public WeatherForecast()
        {
            _name = "Weather Forecast";
            _description = "Today will be a sunny day";
            if (EventHelpers.StoryModeActive)
            {
                //Pick amount of remaining time somewhen between 1 minute and max
                targetTime = (int)Mathf.Lerp(2400, game.world.rainCycle.cycleLength, UnityEngine.Random.Range(0.1f, 1.0f));
                WriteLog(BepInEx.Logging.LogLevel.Debug, $"cycleLength: {game.world.rainCycle.cycleLength}, target: {targetTime}");
                _description = $"Sunshine with heavy rain expected in {Math.Round((float)targetTime / 40f / 60f, 1)} minutes";
            }
        }

        readonly int targetTime = 2400;

        public override void StartupTrigger()
        {
            game.world.rainCycle.timer = (game.world.rainCycle.cycleLength - targetTime);
        }
    }
}
