using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Replaces all creatures in the current room with DLLs, if non found spawns one instead
    /// </summary>
    internal class Rot : CEEvent
    {
        public Rot()
        {
            _name = "The Rot";
            _description = "Eventually everything will be overtaken by it";
        }

        public override void StartupTrigger()
        {
            bool creaturesReplaced = false;
            //Replace all existing creatures with DLLs
            for (int i = EventHelpers.CurrentRoom.creatures.Count - 1; i >= 0; i--)
            {
                AbstractCreature oldCreature = EventHelpers.CurrentRoom.creatures[i];
                if (oldCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat || oldCreature.creatureTemplate.type == CreatureTemplate.Type.Overseer || oldCreature.creatureTemplate.type == CreatureTemplate.Type.Fly) continue;

                WriteLog(LogLevel.Debug, $"Found {oldCreature} , pos: {oldCreature.pos}");
                CreatureTemplate.Type type = CreatureTemplate.Type.DaddyLongLegs;
                AbstractCreature newCreature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(type), null, oldCreature.pos, game.GetNewID());
                WriteLog(LogLevel.Debug, $"Replace creature with {newCreature}");
                EventHelpers.CurrentRoom.AddEntity(newCreature);
                newCreature.Realize();
                newCreature.realizedCreature.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
                oldCreature.realizedCreature.Destroy();
                creaturesReplaced = true;
            }
            //If no creatures found spawn a DLL instead
            if (!creaturesReplaced)
            {
                WriteLog(LogLevel.Debug, "Found no creatures to replace with DLL, spawning one");
                AbstractCreature newCreature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.DaddyLongLegs), null, new WorldCoordinate(game.world.offScreenDen.index, -1, -1, 0), game.GetNewID());
                newCreature.ChangeRooms(EventHelpers.MainPlayer.pos);
            }
        }
    }
}
