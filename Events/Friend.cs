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
            WriteLog(LogLevel.Debug, $"Spawned friendly lizard: {creature}");
        }

        static CreatureTemplate.Type PickLizardType
        {
            get
            {
                int ran = rnd.Next(111);
                if (ModManager.MSC)
                {
                    switch (ran)
                    {
                        case < 10:
                            return CreatureTemplate.Type.GreenLizard;
                        case < 20:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.EelLizard;
                        case < 30:
                            return CreatureTemplate.Type.PinkLizard;
                        case < 40:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SpitLizard;
                        case < 50:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.ZoopLizard;
                        case < 60:
                            return CreatureTemplate.Type.BlueLizard;
                        case < 70:
                            return CreatureTemplate.Type.WhiteLizard;
                        case < 80:
                            return CreatureTemplate.Type.YellowLizard;
                        case < 90:
                            return CreatureTemplate.Type.Salamander;
                        case < 99:
                            return CreatureTemplate.Type.BlackLizard;
                        case < 105:
                            return CreatureTemplate.Type.CyanLizard;
                        case <= 108:
                            return CreatureTemplate.Type.RedLizard;
                        case > 108:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.TrainLizard;

                    }
                }
                else
                {
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
                        case < 99:
                            return CreatureTemplate.Type.BlackLizard;
                        case <= 106:
                            return CreatureTemplate.Type.CyanLizard;
                        case > 106:
                            return CreatureTemplate.Type.RedLizard;
                    }
                }
            }
        }
    }
}
