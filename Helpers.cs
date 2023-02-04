using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainWorldCE
{
    static internal class Helpers
    {
        public static bool ImplementsMethod(this object obj, string name)
        {
            return obj.GetType().GetMethod(name)?.DeclaringType == obj.GetType();
        }

        public static T[] GetAllValues<T>() where T: class
        {
            if (ExtEnum<T>.values == null)
            {
                return new T[0];
            }
            T[] returnArray = new T[ExtEnum<T>.values.entries.Count];
            for (int i = 0; i < ExtEnum<T>.values.entries.Count; i++)
            {
                returnArray[i] = Activator.CreateInstance(typeof(T), new object[] { ExtEnum<T>.values.entries[i], false}) as T;
            }
            return returnArray;
        }
    }
}
