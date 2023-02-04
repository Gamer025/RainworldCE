using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainWorldCE.Config.CustomChaos
{
    internal class CCWait : CCEntry
    {
        private int time;
        public CCWait(int time)
        {
            this.time = time;
        }

        public override int doAction()
        {
            return time;
        }

        public override string ToString()
        {
            return $"Wait for {time} seconds";
        }
    }
}
