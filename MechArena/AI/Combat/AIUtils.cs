using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MechArena.AI.Combat
{
    public static class AIUtils
    {
        public static IEnumerable<SingleClause> DeriveAllClauses()
        {
            return Assembly.GetAssembly(typeof(SingleClause))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SingleClause)))
                .Where(t => !t.IsAbstract)
                .SelectMany(t => ((SingleClause)Activator.CreateInstance(t)).EnumerateClauses());
        }
    }
}

