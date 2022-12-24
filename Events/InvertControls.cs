using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Inverts the players input
    /// </summary>
    internal class InvertControls : CEEvent
    {
        public InvertControls()
        {
            _name = "Directional confusion";
            _description = "Did you try flipping your controler?";
            _activeTime = 120;
        }


        public override void StartupTrigger()
        {
            On.RWInput.PlayerInput += RWInputPlayerInputHook;
        }

        public override void ShutdownTrigger()
        {
            On.RWInput.PlayerInput -= RWInputPlayerInputHook;
        }

        public Player.InputPackage RWInputPlayerInputHook(On.RWInput.orig_PlayerInput orig, int playerNumber, Options options, RainWorldGame.SetupValues setup)
        {
            Player.InputPackage result = orig(playerNumber, options, setup);
            result.x *= -1;
            result.y *= -1;
            return result;
        }
    }
}
