using System;

namespace Griffin.Data;

internal class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string errorMessage) : base(errorMessage)
    {
    }
}