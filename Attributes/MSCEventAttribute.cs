using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
