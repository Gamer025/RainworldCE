using BepInEx.Logging;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Adjust game speed/TPS according to players velocity
    /// </summary>
    internal class MovementTime : CEEvent
    {
        public MovementTime()
        {
            _name = "Super Slugcat";
            _description = "Time only moves... wait wrong game, or is it?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        public override void StartupTrigger()
        {
            On.MainLoopProcess.RawUpdate += MainLoopProcessRawUpdateHook;
        }

        public override void ShutdownTrigger()
        {
            On.MainLoopProcess.RawUpdate -= MainLoopProcessRawUpdateHook;
        }

        public void MainLoopProcessRawUpdateHook(On.MainLoopProcess.orig_RawUpdate orig, MainLoopProcess self, float dt)
        {
            //WriteLog(LogLevel.Info, $"Total vel: {helper.MainPlayer.realizedCreature.mainBodyChunk.vel.y + helper.MainPlayer.realizedCreature.mainBodyChunk.vel.x}");
            self.framesPerSecond = Math.Min(40, Math.Max(10, (int)Math.Abs(EventHelpers.MainPlayer.realizedCreature.mainBodyChunk.vel.y*5) + (int)Math.Abs(EventHelpers.MainPlayer.realizedCreature.mainBodyChunk.vel.x * 10)));
            if (EventHelpers.MainPlayer.realizedCreature.inShortcut) self.framesPerSecond = 30;
            orig(self, dt);
        }
    }
}
