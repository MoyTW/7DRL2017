using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Executor;
using Executor.AI;
using Executor.AI.Combat;
using Executor.AI.Hanger;

namespace Executor.AI
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

        public static Entity AssembleMech(string label, bool player, Entity pilot, IEnumerable<SingleClause> rawRules)
        {
            var steps = rawRules.Where(c => c is Step);
            Entity mech = EntityBuilder.BuildNakedMech(label, player, pilot, new Guidebook(rawRules));
            foreach (Step step in steps)
            {
                step.ApplyStep(mech);
            }
            return mech;
        }
	}
}