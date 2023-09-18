using System.Linq;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Fills up players foodmeter but makes them slightly slower and heavier till next cycle
    /// </summary>
    internal class Thanksgiving : CEEvent
    {
        public Thanksgiving()
        {
            _name = "Thanksgiving";
            _description = "Food up, agility down";
            //Doesn't really do anything, but prevents it from rolling multiple times and lets the player know its there
            _activeTime = 9999;
        }

        public override void StartupTrigger()
        {
            //Resets itself when cycle ends
            (EventHelpers.MainPlayer.realizedCreature as Player).AddFood(
                (EventHelpers.MainPlayer.realizedCreature as Player).MaxFoodInStomach - (EventHelpers.MainPlayer.realizedCreature as Player).FoodInStomach);
            if (ModManager.CoopAvailable)
            {
                foreach (SlugcatStats stats in (game.session as StoryGameSession).characterStatsJollyplayer.Where(x => x is not null))
                {
                    stats.bodyWeightFac += 0.20f;
                    stats.runspeedFac -= 0.20f;
                }
            }
            else
            {
                game.session.characterStats.bodyWeightFac += 0.20f;
                game.session.characterStats.runspeedFac -= 0.20f;
            }
        }
    }
}
