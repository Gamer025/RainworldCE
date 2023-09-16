using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Makes the player loose grasp of an item after about 7 seconds if they don't switch hands/replace/drop it
    /// </summary>
    internal class LooseGrasp : CEEvent
    {
        public LooseGrasp()
        {
            _name = "Hot Potatoes";
            _description = "Why is everything suddenly hot to the touch?\n Holding stuff for too long might burn you";
            _repeatEverySec = 1;
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        Tuple<Creature.Grasp[], Creature.Grasp[], int[]>[] graspCounter = new Tuple<Creature.Grasp[], Creature.Grasp[], int[]>[16]; //4 players should be max but just in case?
        public override void RecurringTrigger()
        {
            for (int playerIndex = 0; playerIndex < EventHelpers.AllPlayers.Count; playerIndex++)
            {
                AbstractCreature playerA = EventHelpers.AllPlayers[playerIndex];
                Creature player = playerA.realizedCreature;
                if (graspCounter[playerIndex] is null)
                {
                    graspCounter[playerIndex] = new (player.grasps, (Creature.Grasp[])player.grasps.Clone(), new int[player.grasps.Length]);
                }
                for (int graspIndex = 0; graspIndex < graspCounter[playerIndex].Item1.Length; graspIndex++)
                {
                    if (ReferenceEquals(graspCounter[playerIndex].Item1[graspIndex], graspCounter[playerIndex].Item2[graspIndex]))
                    {
                        graspCounter[playerIndex].Item3[graspIndex] += 1;
                    }
                    else
                    {
                        graspCounter[playerIndex].Item3[graspIndex] = 0;
                        graspCounter[playerIndex].Item2[graspIndex] = graspCounter[playerIndex].Item1[graspIndex];
                    }

                    if (graspCounter[playerIndex].Item3[graspIndex] > 6)
                    {
                        (player as Player).ReleaseGrasp(graspIndex);
                    }
                }
            }
        }
    }
}
