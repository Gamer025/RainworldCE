using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE
{
    static internal class ExtensionMethods
    {
        public static bool ImplementsMethod(this object obj, string name)
        {
            return obj.GetType().GetMethod(name)?.DeclaringType == obj.GetType();
        }
    }
}
