namespace RainWorldCE.Events
{
    /// <summary>
    /// Randomly decreases or increases Karma by 1
    /// </summary>
    internal class KarmaLevel : CEEvent
    {
        public KarmaLevel()
        {
            _name = "Karma shuffle";
            _description = "Some people might call this 'Instant Karma'";
        }

        public override void StartupTrigger()
        {
            int[] possibleValues = new int[2] { -1, 1 };
            int result = possibleValues[rnd.Next(possibleValues.Length)];
            if ((game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == 0)
                result = 1;
            if ((game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == (game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap)
                result = -1;

            (game.session as StoryGameSession).saveState.deathPersistentSaveData.karma += result;
            foreach (RoomCamera camera in game.cameras)
            {
                if (camera.hud.karmaMeter != null)
                {
                    camera.hud.karmaMeter.forceVisibleCounter = 100;
                    camera.hud.karmaMeter.UpdateGraphic();
                }
            }
            //Spark
            for (int amount = 0; amount < 20; amount++)
                EventHelpers.CurrentRoom.realizedRoom.AddObject(new Spark(EventHelpers.MainPlayer.realizedCreature.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value * 40f, new UnityEngine.Color(1f, 1f, 1f), null, 30, 120));
        }
    }
}
