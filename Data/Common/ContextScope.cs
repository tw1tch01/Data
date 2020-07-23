using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.Common
{
    public class ContextScope
    {
        public ContextScope()
        {
            StateActions = new Dictionary<EntityState, Action<EntityEntry>>();
        }

        public Dictionary<EntityState, Action<EntityEntry>> StateActions { get; }
    }
}