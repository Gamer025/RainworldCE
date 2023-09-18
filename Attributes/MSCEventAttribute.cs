using System;

namespace RainWorldCE.Attributes
{
    /// <summary>
    /// These events only work with MSC enabled
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class MSCEventAttribute : System.Attribute
    {
    }
}
