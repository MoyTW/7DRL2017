using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public static class ParentStrategies
    {
        public static Individual<T> Roulette<T>(Population<T> pop, Random rand)
        {
            if (pop.CurrentSize < 2)
                throw new InvalidOperationException("Can't select from an undersize population! (pop < 2)");

            int totalweight = pop.InspectIndividuals().Sum(i => i.Fitness);
            int choice = rand.Next(totalweight);
            int weightIndex = 0;
            
            foreach (var individual in pop.InspectIndividuals())
            {
                weightIndex += individual.Fitness;

                if (weightIndex > choice)
                    return individual;
            }

            throw new InvalidOperationException("There's some bug in your picker code!");
        }
    }
}
