using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SlugcatStats;
using UnityEngine;
using BepInEx.Logging;
using System.Drawing;

namespace RainWorldCE.Events
{

    //Catapults all players into the air
    internal class LiftOff : CEEvent
    {
        public LiftOff()
        {
            _name = "Lift off";
            _description = "in T-10 seconds";
            _activeTime = 30;
            _repeatEverySec = 10;
            _allowMultiple = true;
        }

        public override void RecurringTrigger()
        {
            if (ModManager.MSC)
            {
                VirtualMicrophone virtMic = game.cameras[0].virtualMicrophone;
                SoundLoader.SoundData soundData = virtMic.GetSoundData(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Inv_Hit, 2);
                if (virtMic.SoundClipReady(soundData))
                {
                    virtMic.soundObjects.Add(new VirtualMicrophone.DisembodiedSound(virtMic, soundData, 0f, 1f, 1f, startAtRandomTime: false, 0));
                }
            }

            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                player.realizedCreature.mainBodyChunk.vel.y += 100f;
            }
        }
    }
}
