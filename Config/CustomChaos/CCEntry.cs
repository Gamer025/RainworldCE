using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainWorldCE.Config.CustomChaos
{
    internal abstract class CCEntry
    {
        public abstract int doAction();

        public abstract override string ToString();
    }
}
