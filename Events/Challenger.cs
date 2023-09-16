using BepInEx.Logging;
using IL.LizardCosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainWorldCE.Events
{
    internal class Challenger : CEEvent
    {
        /// <summary>
        /// Spawns a random enemy
        /// </summary>
        public Challenger()
        {
            _name = "Challenger";
            _description = "A new foe has appeared";
        }

        public override void StartupTrigger()
        {
            CreatureTemplate.Type creatureType = RandomCreature(EventHelpers.CurrentRoom.AnySkyAccess);
            WriteLog(LogLevel.Debug, $"Spawning: {creatureType}");
            List<AbstractRoom> adjacendRooms = EventHelpers.GetConnectedRooms(EventHelpers.CurrentRoom);
            AbstractRoom sourceRoom = adjacendRooms[rnd.Next(adjacendRooms.Count)];
            int destRoomNodeId = EventHelpers.GetNodeIdOfRoomConnection(sourceRoom, EventHelpers.CurrentRoom);
            if (creatureType == CreatureTemplate.Type.Vulture || creatureType == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.MirosVulture)
            {
                sourceRoom = game.world.offScreenDen;
                for (int i = 0; i < EventHelpers.CurrentRoom.realizedRoom.borderExits.Length; i++)
                {
                    if (!(EventHelpers.CurrentRoom.realizedRoom.borderExits[i].type == AbstractRoomNode.Type.SkyExit))
                    {
                        continue;
                    }
                    destRoomNodeId = i + EventHelpers.CurrentRoom.realizedRoom.exitAndDenIndex.Length;
                }
            }
            int amt = 1;
            if (creatureType == CreatureTemplate.Type.Spider)
                amt = 16;
            for (int i = 0; i < amt; i++)
            {
                AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(creatureType), null, new WorldCoordinate(sourceRoom.index, -1, -1, 0), game.GetNewID());
                if (creatureType == CreatureTemplate.Type.Centipede)
                {
                    creature.spawnData = $"{{{UnityEngine.Random.Range(0.6f, 1.0f)}}}";
                }
                EventHelpers.MakeCreatureAttackCreature(creature, EventHelpers.MainPlayer);
                creature.ChangeRooms(new WorldCoordinate(EventHelpers.CurrentRoom.index, -1, -1, destRoomNodeId));
                WriteLog(LogLevel.Debug, $"Spawned: {creature}");
                if (creatureType == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.Inspector)
                {
                    creature.abstractAI.RealAI.preyTracker.AddPrey(creature.abstractAI.RealAI.tracker.RepresentationForCreature(EventHelpers.MainPlayer, true));
                    (creature.realizedCreature as MoreSlugcats.Inspector).anger = 1.0f;
                }
            }
        }

        public static CreatureTemplate.Type RandomCreature(bool allowFlying = false)
        {
            int ran = rnd.Next(111);
            if (rnd.Next(100) > 50)
            {
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
                        case < 198:
                            return CreatureTemplate.Type.RedLizard;
                        case >= 109:
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
                        case < 107:
                            return CreatureTemplate.Type.CyanLizard;
                        case >= 107:
                            return CreatureTemplate.Type.RedLizard;
                    }
                }

            }
            else
            {
                ran = rnd.Next(120);
                if (ModManager.MSC)
                {
                    switch (ran)
                    {
                        case < 15:
                            return CreatureTemplate.Type.Centipede;
                        case < 30:
                            return CreatureTemplate.Type.BigSpider;
                        case < 40:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.MotherSpider;
                        case < 50:
                            if (allowFlying) return CreatureTemplate.Type.Vulture;
                            else return RandomCreature();
                        case < 60:
                            return CreatureTemplate.Type.Spider;
                        case < 70:
                            return CreatureTemplate.Type.SpitterSpider;
                        case < 80:
                            return CreatureTemplate.Type.Centiwing;
                        case < 87:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite;
                        case < 95:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.FireBug;
                        case < 101:
                            return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.Inspector;
                        case < 108:
                            return CreatureTemplate.Type.MirosBird;
                        case < 114:
                            if (allowFlying) return MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.MirosVulture;
                            else return RandomCreature();
                        case >= 114:
                            return CreatureTemplate.Type.RedCentipede;

                    }
                }
                else
                {
                    switch (ran)
                    {
                        case < 20:
                            return CreatureTemplate.Type.Centipede;
                        case < 40:
                            return CreatureTemplate.Type.BigSpider;
                        case < 55:
                            if (allowFlying) return CreatureTemplate.Type.Vulture;
                            else return RandomCreature();
                        case < 70:
                            return CreatureTemplate.Type.Spider;
                        case < 86:
                            return CreatureTemplate.Type.SpitterSpider;
                        case < 100:
                            return CreatureTemplate.Type.Centiwing;
                        case < 110:
                            return CreatureTemplate.Type.MirosBird;
                        case >= 110:
                            return CreatureTemplate.Type.RedCentipede;
                    }
                }
            }
        }
    }
}


