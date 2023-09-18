using System;
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
        }

        public override void StartupTrigger()
        {
            //Pick amount of remaining time somewhen between 1 minute and max
            int targetTime = (int)Mathf.Lerp(2400, game.world.rainCycle.cycleLength, UnityEngine.Random.Range(0.1f, 1.0f));
            WriteLog(BepInEx.Logging.LogLevel.Debug, $"cycleLength: {game.world.rainCycle.cycleLength}, target: {targetTime}");
            _description = $"Sunshine with heavy rain expected in {Math.Round((float)targetTime / 40f / 60f, 1)} minutes";
            game.world.rainCycle.timer = (game.world.rainCycle.cycleLength - targetTime);
        }
    }
}
