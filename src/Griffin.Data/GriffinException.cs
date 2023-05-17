using System;
using System.Collections.Generic;
using System.Text;

namespace Griffin.Data
{
    /// <summary>
    /// Just an exception used to let us identify all our exceptions (so that we don't wrap them).
    /// </summary>
    public class GriffinException : Exception
    {
        protected GriffinException()
        {

        }

        protected GriffinException(string message, Exception inner)
        : base(message, inner)
        {

        }

        protected GriffinException(string errorMessage)
        : base(errorMessage)
        {

        }
    }
}
