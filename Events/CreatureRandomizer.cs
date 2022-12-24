using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Randomizes all creatures when the players enters a room
    /// </summary>
    internal class CreatureRandomizer : CEEvent
    {
        public CreatureRandomizer()
        {
            _name = "DNA Mutations";
            _description = "Was that thing always here?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        readonly CreatureTemplate.Type[] possibleCreatures = {
            CreatureTemplate.Type.PinkLizard,
            CreatureTemplate.Type.GreenLizard,
            CreatureTemplate.Type.BlueLizard,
            CreatureTemplate.Type.YellowLizard,
            CreatureTemplate.Type.WhiteLizard,
            CreatureTemplate.Type.RedLizard,
            CreatureTemplate.Type.BlackLizard,
            CreatureTemplate.Type.Salamander,
            CreatureTemplate.Type.CyanLizard,
            CreatureTemplate.Type.Fly,
            CreatureTemplate.Type.Leech,
            CreatureTemplate.Type.SeaLeech,
            CreatureTemplate.Type.Snail,
            CreatureTemplate.Type.Vulture,
            CreatureTemplate.Type.LanternMouse,
            CreatureTemplate.Type.CicadaA,
            CreatureTemplate.Type.CicadaB,
            CreatureTemplate.Type.Spider,
            CreatureTemplate.Type.JetFish,
            CreatureTemplate.Type.BigEel,
            CreatureTemplate.Type.TubeWorm,
            CreatureTemplate.Type.DaddyLongLegs,
            CreatureTemplate.Type.BrotherLongLegs,
            CreatureTemplate.Type.TentaclePlant,
            CreatureTemplate.Type.PoleMimic,
            CreatureTemplate.Type.MirosBird,
            CreatureTemplate.Type.TempleGuard,
            CreatureTemplate.Type.Centipede,
            CreatureTemplate.Type.RedCentipede,
            CreatureTemplate.Type.Centiwing,
            CreatureTemplate.Type.SmallCentipede,
            CreatureTemplate.Type.Scavenger,
            CreatureTemplate.Type.Overseer,
            CreatureTemplate.Type.VultureGrub,
            CreatureTemplate.Type.EggBug,
            CreatureTemplate.Type.BigSpider,
            CreatureTemplate.Type.SpitterSpider,
            CreatureTemplate.Type.SmallNeedleWorm,
            CreatureTemplate.Type.BigNeedleWorm,
            CreatureTemplate.Type.DropBug,
            CreatureTemplate.Type.KingVulture,
            CreatureTemplate.Type.Hazer };

        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            AbstractRoom movingTo = room.abstractRoom;
            for (int i = movingTo.creatures.Count - 1; i >= 0; i--)
            {
                AbstractCreature oldCreature = movingTo.creatures[i];
                if (oldCreature.creatureTemplate.type is CreatureTemplate.Type.Slugcat or CreatureTemplate.Type.Overseer or CreatureTemplate.Type.Deer) continue;

                WriteLog(LogLevel.Debug, $"Found {oldCreature} , pos: {oldCreature.pos}");
                CreatureTemplate.Type type = (CreatureTemplate.Type)possibleCreatures.GetValue(rnd.Next(possibleCreatures.Length));
                AbstractCreature newCreature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(type), null, oldCreature.pos, game.GetNewID());
                WriteLog(LogLevel.Debug, $"Replace creature with {newCreature}");
                movingTo.AddEntity(newCreature);
                newCreature.Realize();
                newCreature.realizedCreature.PlaceInRoom(room);
                oldCreature.realizedCreature.Destroy();
            }
        }
    }
}
