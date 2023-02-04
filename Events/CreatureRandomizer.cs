using BepInEx.Logging;
using IL;
using IL.JollyCoop.JollyManual;
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

        List<CreatureTemplate.Type> possibleCreatures = new List<CreatureTemplate.Type>();
        public CreatureRandomizer()
        {
            _name = "DNA Mutations";
            _description = "Was that thing always here?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
            CreatureTemplate.Type[] creatureTypes = Helpers.GetAllValues<CreatureTemplate.Type>();
            possibleCreatures.AddRange(creatureTypes);
            possibleCreatures.Remove(CreatureTemplate.Type.Deer);
            possibleCreatures.Remove(CreatureTemplate.Type.LizardTemplate);
            possibleCreatures.Remove(CreatureTemplate.Type.Overseer);
            possibleCreatures.Remove(CreatureTemplate.Type.Slugcat);
            possibleCreatures.Remove(CreatureTemplate.Type.StandardGroundCreature);
            possibleCreatures.Remove(CreatureTemplate.Type.GarbageWorm);
            if (ModManager.MSC)
            {
                possibleCreatures.Remove(MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.StowawayBug);
            }
        }


        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            AbstractRoom movingTo = room.abstractRoom;
            for (int i = movingTo.creatures.Count - 1; i >= 0; i--)
            {
                AbstractCreature oldCreature = movingTo.creatures[i];
                if (oldCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat || oldCreature.creatureTemplate.type == CreatureTemplate.Type.Overseer || oldCreature.creatureTemplate.type == CreatureTemplate.Type.Deer) continue;

                WriteLog(LogLevel.Debug, $"Found {oldCreature} , pos: {oldCreature.pos}");
                CreatureTemplate.Type type = possibleCreatures[rnd.Next(possibleCreatures.Count)];
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
