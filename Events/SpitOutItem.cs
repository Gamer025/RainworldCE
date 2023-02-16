using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Makes the player spit out the current item
    /// </summary>
    internal class SpitOutItem : CEEvent
    {
        public SpitOutItem()
        {
            _name = "Stomach ache";
            _description = "Maybe eating random things is bad even for Slugcats";
        }

        public override void StartupTrigger()
        {
            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                if ((player.realizedCreature as Player).SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear)
                {
                    AbstractSpear spear = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), explosive: false);
                    EventHelpers.CurrentRoom.AddEntity(spear);
                    spear.pos = player.pos;
                    spear.RealizeInRoom();
                    (spear.realizedObject as Spear).Spear_makeNeedle(UnityEngine.Random.Range(0, 3), active: true);
                    if ((player.realizedCreature as Player).FreeHand() > -1)
                    {
                        (player.realizedCreature as Player).SlugcatGrab(spear.realizedObject, (player.realizedCreature as Player).FreeHand());
                    }
                }
                else
                    (player.realizedCreature as Player).Regurgitate();
            }
        }
    }
}
