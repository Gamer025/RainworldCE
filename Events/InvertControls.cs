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
            On.RWInput.PlayerInputLogic_int_int += RWInputPlayerInputHook;
        }

        public override void ShutdownTrigger()
        {
            On.RWInput.PlayerInputLogic_int_int -= RWInputPlayerInputHook;
        }

        public Player.InputPackage RWInputPlayerInputHook(On.RWInput.orig_PlayerInputLogic_int_int orig, int categoryID, int playerNumber)
        {
            Player.InputPackage result = orig(categoryID, playerNumber);
            //Don't invert controls in the menu
            if (game?.pauseMenu == null)
            {
                result.x *= -1;
                result.y *= -1;
            }
            return result;
        }
    }
}
