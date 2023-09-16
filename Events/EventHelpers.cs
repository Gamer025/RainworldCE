using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Helper methods for event related stuff
    /// </summary>
    public static class EventHelpers
    {

        /// <summary>
        /// Creates a instance of every event and pulls out its name
        /// </summary>
        /// <returns>List of all existing event names</returns>
        internal static List<string> GetAllEventNames()
        {
            List<string> returnList = new List<string>();
            List<Type> eventTypes = RainWorldCE.GetAllCEEventTypes().OrderBy(x => x.Name).ToList();
            foreach (Type type in eventTypes)
            {
                CEEvent ceevent = (CEEvent)Activator.CreateInstance(type);
                returnList.Add(ceevent.Name);
            }
            return returnList;
        }

        /// <summary>
        /// Gets a random abstract room in the current region
        /// </summary>
        /// <param name="onlySafeRooms">Only return safe rooms (no offScreenDens, Gates or Rooms without connections)</param>
        /// <returns>Abstract room from the current region</returns>
        internal static AbstractRoom RandomRegionRoom(bool onlySafeRooms = true)
        {
            bool foundValidRoom = false;
            AbstractRoom aRoom = null;
            while (!foundValidRoom)
            {
                int roomID = CEEvent.rnd.Next(CEEvent.game.world.firstRoomIndex, CEEvent.game.world.firstRoomIndex + CEEvent.game.world.NumberOfRooms);
                aRoom = CEEvent.game.world.GetAbstractRoom(roomID);
                //Room is always valid if onlySafeRooms false or room has connections and is not gate or offScreenDen
                foundValidRoom = !onlySafeRooms || (!aRoom.offScreenDen && aRoom.connections.Length > 0 && !aRoom.name.Contains("GATE"));
            }
            return aRoom;
        }

        /// <summary>
        /// Returns the first player in game.players
        /// </summary>
        /// <returns></returns>
        internal static AbstractCreature MainPlayer
        {
            get
            {
                //Get first alive player
                foreach (AbstractCreature player in CEEvent.game.Players)
                {
                    if (!player?.realizedCreature.dead ?? false)
                    {
                        return player;
                    }
                }
                //Failsafe
                return CEEvent.game.Players[0];
            }
        }

        /// <summary>
        /// Returns all players in game.players
        /// </summary>
        /// <returns></returns>
        internal static Collection<AbstractCreature> AllPlayers
        {
            get
            {
                return new Collection<AbstractCreature>(CEEvent.game.Players);
            }
        }

        internal static AbstractRoom CurrentRoom
        {
            get
            {
                return MainPlayer.Room;
            }
        }

        internal static Collection<AbstractRoom> AllRegionRooms
        {
            get
            {
                Collection<AbstractRoom> rooms = new Collection<AbstractRoom>();
                for (int roomID = CEEvent.game.world.NumberOfRooms - 1; roomID >= 0; roomID--)
                {
                    rooms.Add(CEEvent.game.world.GetAbstractRoom(roomID + CEEvent.game.world.firstRoomIndex));
                }
                return rooms;
            }
        }

        internal static List<AbstractRoom> GetConnectedRooms(AbstractRoom room)
        {
            List<AbstractRoom> returnList = new List<AbstractRoom>();
            foreach (var destRoomNumber in room.connections)
            {
                if (destRoomNumber > 1) returnList.Add(CEEvent.game.world.GetAbstractRoom(destRoomNumber));
            }
            return returnList;
        }

        internal static int GetNodeIdOfRoomConnection(AbstractRoom source, AbstractRoom destination)
        {
            return Array.IndexOf(destination.connections, source.index);
        }

        internal static void MakeCreatureAttackCreature(AbstractCreature attacker, AbstractCreature target)
        {
            if (attacker.state?.socialMemory is not null)
            {
                attacker.state.socialMemory.GetOrInitiateRelationship(target.ID).like = -1f;
                attacker.state.socialMemory.GetOrInitiateRelationship(target.ID).tempLike = -1f;
            }
            attacker.personality.aggression = 1f;
            attacker.personality.dominance = 1f;
            attacker.personality.bravery = 1f;
            if (attacker.abstractAI is not null)
            {
                attacker.abstractAI.followCreature = target;
            }
        }

        internal static void MakeCreatureLikeAndFollowCreature(AbstractCreature friend, AbstractCreature target)
        {
            friend.state.socialMemory.GetOrInitiateRelationship(target.ID).like = 1f;
            friend.state.socialMemory.GetOrInitiateRelationship(target.ID).tempLike = 1f;
            friend.abstractAI.followCreature = target;
        }

        internal static bool RoomHasEffect(Room room, RoomSettings.RoomEffect.Type type)
        {
            foreach (RoomSettings.RoomEffect effect in room.roomSettings.effects)
            {
                if (effect.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool StoryModeActive
        {
            get
            {
                return (CEEvent.game is not null && CEEvent.game.IsStorySession);
            }
        }
    }
}
