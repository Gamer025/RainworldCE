using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a random friendly lizard that follows the player
    /// </summary>
    internal class Friend : CEEvent
    {
        public Friend()
        {
            _name = "The Friend";
            _description = "One need not travel alone";
        }

        public override void StartupTrigger()
        {
            AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(PickLizardType), null, EventHelpers.MainPlayer.pos, game.GetNewID());
            EventHelpers.MakeCreatureLikeAndFollowCreature(creature, EventHelpers.MainPlayer);
            creature.Realize();
            creature.realizedCreature.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
        }

        CreatureTemplate.Type PickLizardType
        {
            get
            {
                int ran = rnd.Next(100);
                switch (ran)
                {
                    case < 20:
                        return CreatureTemplate.Type.GreenLizard;
                    case < 40:
                        return CreatureTemplate.Type.PinkLizard;
                    case < 60:
                        return CreatureTemplate.Type.BlueLizard;
                    case < 70:
                        return CreatureTemplate.Type.WhiteLizard;
                    case < 80:
                        return CreatureTemplate.Type.YellowLizard;
                    case < 90:
                        return CreatureTemplate.Type.Salamander;
                    case < 96:
                        return CreatureTemplate.Type.BlackLizard;
                    case <= 98:
                        return CreatureTemplate.Type.CyanLizard;
                    case > 98:
                        return CreatureTemplate.Type.RedLizard;

                }
            }
        }
    }
}
