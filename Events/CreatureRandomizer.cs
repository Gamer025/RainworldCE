using BepInEx.Logging;
using RainWorldCE.Config;
using System.Collections.Generic;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Randomizes all creatures when the players enters a room
    /// </summary>
    internal class CreatureRandomizer : CEEvent
    {
        Dictionary<AbstractCreature, AbstractCreature> restore = new Dictionary<AbstractCreature, AbstractCreature>();
        public CreatureRandomizer()
        {
            _name = "DNA Mutations";
            _description = "Was that thing always here?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }


        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            List<CreatureTemplate.Type> possibleCreatures = new List<CreatureTemplate.Type>();
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
                if (TryGetConfigAsBool("excludePups"))
                    possibleCreatures.Remove(MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC);
            }

            AbstractRoom movingTo = room.abstractRoom;
            for (int i = movingTo.creatures.Count - 1; i >= 0; i--)
            {
                AbstractCreature oldCreature = movingTo.creatures[i];
                if (oldCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat ||
                    oldCreature.creatureTemplate.type == CreatureTemplate.Type.Overseer ||
                    oldCreature.creatureTemplate.type == CreatureTemplate.Type.Deer ||
                    (oldCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC && TryGetConfigAsBool("excludePups"))) continue;

                WriteLog(LogLevel.Debug, $"Found {oldCreature} , pos: {oldCreature.pos}");
                CreatureTemplate.Type type = possibleCreatures[rnd.Next(possibleCreatures.Count)];
                AbstractCreature newCreature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(type), null, oldCreature.pos, game.GetNewID());
                WriteLog(LogLevel.Debug, $"Replace creature with {newCreature}");
                movingTo.AddEntity(newCreature);
                newCreature.Realize();
                newCreature.realizedCreature.PlaceInRoom(room);
                //If list already contains the creature we are now replacing it means that creature itself is a randomized creature
                //Get that real original creature and add it with the new creature as key
                if (TryGetConfigAsBool("restoreCreatures") && restore.ContainsKey(oldCreature))
                {
                    restore.Add(newCreature, restore[oldCreature]);
                    restore.Remove(oldCreature);
                    oldCreature.realizedCreature.Destroy();
                }
                //Otherwise just back it up
                else if (TryGetConfigAsBool("restoreCreatures"))
                {
                    oldCreature.realizedCreature.room = null;
                    oldCreature.Abstractize(oldCreature.pos);
                    oldCreature.Room.RemoveEntity(oldCreature.ID);
                    if (oldCreature is not null && !oldCreature.slatedForDeletion)
                    {
                        restore.Add(newCreature, oldCreature);
                    }
                    else
                    {
                        restore.Add(newCreature, null);
                    }
                }
                else
                    oldCreature.realizedCreature.Destroy();
            }
        }

        public override void ShutdownTrigger()
        {
            if (TryGetConfigAsBool("restoreCreatures"))
            {
                WriteLog(LogLevel.Debug, "Restoring creatures...");
                foreach (KeyValuePair<AbstractCreature, AbstractCreature> kvp in restore)
                {
                    //If value (orig creature) is null just delete the replacement
                    if (kvp.Value is null)
                    {
                        if (kvp.Key.realizedCreature is not null)
                        {
                            kvp.Key.realizedCreature.Destroy();
                        }
                        else
                        {
                            kvp.Key.Room.RemoveEntity(kvp.Key);
                            kvp.Key.Destroy();
                        }
                        return;
                    }

                    //Otherwise restoer the orig creature and delete replacemnet
                    WriteLog(LogLevel.Debug, $"Restoring {kvp.Value} with {kvp.Key} as randomized creature");
                    kvp.Value.pos = kvp.Key.pos;
                    kvp.Key.Room.AddEntity(kvp.Value);
                    if (kvp.Key.realizedCreature is not null)
                    {
                        if (kvp.Key.realizedCreature.room is not null)
                        {
                            kvp.Value.Realize();
                            kvp.Value.realizedCreature.PlaceInRoom(kvp.Key.realizedCreature.room);
                        }
                        else
                        {
                            WriteLog(LogLevel.Debug, $"Realized creature room null, try abstract: {kvp.Key.Room.realizedRoom}");
                            kvp.Value.Realize();
                            kvp.Value.realizedCreature.PlaceInRoom(kvp.Key.Room.realizedRoom);
                        }
                        kvp.Key.realizedCreature.Destroy();
                    }
                    else
                        kvp.Key.Room.RemoveEntity(kvp.Key);
                    kvp.Key.Destroy();
                }
            }
        }

        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new BooleanConfigEntry("Restore creatures?", "Restore the original creatures at the end of the event?", "restoreCreatures", false, this),
                    new BooleanConfigEntry("Exclude slugpups?", "Excludes slugpups from being randomized?", "excludePups", true, this)
                };
                return options;
            }
        }
    }
}
