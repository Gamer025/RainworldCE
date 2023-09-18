using BepInEx.Logging;
using System;

namespace RainWorldCE.Config.CustomChaos
{
    internal static class CustomChaos
    {
        /// <summary>
        /// Current config parsed to events
        /// </summary>
        public static CCEntry[] CCConfig;
        /// <summary>
        /// How many entries have been run, used to keep track of next entry to run
        /// </summary>
        public static int step;
        /// <summary>
        /// gameTime at which to run next entry
        /// </summary>
        public static int nextTrigger = 0;

        static public void CustomChaosUpdate()
        {
            int loop = 0;
            while (nextTrigger <= RainWorldCE.gameTimer)
            {
                RainWorldCE.ME.Logger_p.Log(LogLevel.Debug, $"[CustomChaos] Triggering {CCConfig[step]}");
                nextTrigger = CCConfig[step].doAction() + RainWorldCE.gameTimer;
                step++;
                if (step >= CCConfig.Length)
                {
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Info, "[CustomChaos] Reached end of script, exiting");
                    nextTrigger = Int32.MaxValue;
                }
                    
                loop++;
                if (loop > 100)
                {
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, "[CustomChaos] Possible loop detected aborting. Make sure to add WAIT statements inside your goto loop");
                    nextTrigger = Int32.MaxValue;
                }
            }
        }

        public static void parseConfig(string[] config)
        {
            CCConfig = new CCEntry[config.Length];
            for (int i = 0; i < config.Length; i++)
            {
                string[] parsed = config[i].Split(' ');
                try
                {
                    switch (parsed[0])
                    {
                        case "WAIT":
                            CCConfig[i] = new CCWait(Int32.Parse(parsed[1]));
                            break;
                        case "GOTO":
                            if (parsed[1] is not null && Int32.Parse(parsed[1]) - 1 < config.Length)
                                CCConfig[i] = new CCGoto(Int32.Parse(parsed[1]));
                            else
                                CCConfig[i] = new CCError();
                            break;
                        case "RANDOM":
                            CCConfig[i] = new CCRandomEvent();
                            break;
                        default:
                            int time = 0;
                            if (parsed.Length > 1)
                                time = Int32.Parse(parsed[1]);
                            CCConfig[i] = new CCExecuteEvent(Type.GetType($"RainWorldCE.Events.{parsed[0]}"), time);
                            break;
                    }
                }
                catch (Exception e)
                {
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, "[CustomChaos] Error parsing cc.txt:");
                    RainWorldCE.ME.Logger_p.Log(LogLevel.Error, e.Message);
                    CCConfig[i] = new CCError();
                }
            }
        }
    }
}
