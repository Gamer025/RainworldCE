using System;

namespace RainWorldCE.Attributes
{
    /// <summary>
    /// These events should never be randomly rolled
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class InternalCEEventAttribute : System.Attribute 
    {
    }
}
