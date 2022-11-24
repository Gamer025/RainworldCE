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
    public class EventHelpers
    {
        private readonly RainWorldGame game;

        public EventHelpers(RainWorldGame game)
        {
            this.game = game;
        }

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

        internal AbstractRoom RandomRegionRoom
        {
            get
            {
                //Last room should be offscreenDen so -1
                int roomID = CEEvent.rnd.Next(game.world.firstRoomIndex, game.world.firstRoomIndex + game.world.NumberOfRooms-1);
                AbstractRoom aRoom = CEEvent.game.world.GetAbstractRoom(roomID);
                return aRoom;
            }
        }
        /// <summary>
        /// Returns the first player in game.players
        /// </summary>
        /// <returns></returns>
        internal AbstractCreature MainPlayer
        {
            get
            {
                return game.Players[0];
            }
        }

        /// <summary>
        /// Returns all players in game.players
        /// </summary>
        /// <returns></returns>
        internal Collection<AbstractCreature> AllPlayers
        {
            get
            {
                return new Collection<AbstractCreature>(game.Players);
            }
        }

        internal AbstractRoom CurrentRoom
        {
            get
            {
                return MainPlayer.Room;
            }
        }

        internal Collection<AbstractRoom> AllRegionRooms
        {
            get
            {
                Collection<AbstractRoom> rooms = new Collection<AbstractRoom>();
                for (int roomID = game.world.NumberOfRooms - 1; roomID >= 0; roomID--)
                {
                    rooms.Add(game.world.GetAbstractRoom(roomID + game.world.firstRoomIndex));
                }
                return rooms;
            }
        }

        internal List<AbstractRoom> GetConnectedRooms(AbstractRoom room)
        {
            List<AbstractRoom> returnList = new List<AbstractRoom>();
            foreach (var destRoomNumber in room.connections)
            {
                if (destRoomNumber > 1) returnList.Add(game.world.GetAbstractRoom(destRoomNumber));
            }
            return returnList;
        }

        internal int GetNodeIdOfRoomConnection(AbstractRoom source, AbstractRoom destination)
        {
            return Array.IndexOf(destination.connections, source.index);
        }

        internal void MakeCreatureAttackCreature(AbstractCreature attacker, AbstractCreature target)
        {
            attacker.state.socialMemory.GetOrInitiateRelationship(target.ID).like = -1f;
            attacker.state.socialMemory.GetOrInitiateRelationship(target.ID).tempLike = -1f;
            attacker.personality.aggression = 1f;
            attacker.personality.dominance = 1f;
            attacker.personality.bravery = 1f;
            attacker.abstractAI.followCreature = target;
        }

        internal void MakeCreatureLikeAndFollowCreature(AbstractCreature friend, AbstractCreature target)
        {
            friend.state.socialMemory.GetOrInitiateRelationship(target.ID).like = 1f;
            friend.state.socialMemory.GetOrInitiateRelationship(target.ID).tempLike = 1f;
            friend.abstractAI.followCreature = target;
        }

        internal bool RoomHasEffect(Room room, RoomSettings.RoomEffect.Type type)
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
    }
}
