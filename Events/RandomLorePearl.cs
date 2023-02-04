using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RainWorldCE;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a random lore pearl on the player
    /// </summary>
    internal class RandomLorePearl : CEEvent
    {
        public RandomLorePearl()
        {
            _name = "A piece of history";
            _description = "Maybe you haven't yet heard this one";
        }

        public override void StartupTrigger()
        {
            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                AbstractPhysicalObject reward;
                DataPearl.AbstractDataPearl.DataPearlType[] pearls = Helpers.GetAllValues<DataPearl.AbstractDataPearl.DataPearlType>();
                DataPearl.AbstractDataPearl.DataPearlType[] badPearls = new DataPearl.AbstractDataPearl.DataPearlType[3]
                {
                    DataPearl.AbstractDataPearl.DataPearlType.PebblesPearl,
                    DataPearl.AbstractDataPearl.DataPearlType.Misc,
                    DataPearl.AbstractDataPearl.DataPearlType.Misc2
                };
                //Remove problematic/generic pearls
                pearls = pearls.Where(val => !badPearls.Contains(val)).ToArray();
                DataPearl.AbstractDataPearl.DataPearlType pearlType = (DataPearl.AbstractDataPearl.DataPearlType)pearls.GetValue(rnd.Next(pearls.Length));
                WriteLog(BepInEx.Logging.LogLevel.Debug, $"PearlType: {pearlType}");
                reward = new DataPearl.AbstractDataPearl(game.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, player.pos, game.GetNewID(), -1, -1, null, pearlType);

                reward.Realize();
                reward.realizedObject.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
            }
        }
    }
}
