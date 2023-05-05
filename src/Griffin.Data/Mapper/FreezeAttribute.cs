using System;
using System.Collections.Generic;
using System.Text;

namespace Griffin.Data.Mapper
{
    /// <summary>
    /// Put this attribute on your classes to prevent the scaffolder to override them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FreezeAttribute : Attribute
    {
    }
}
