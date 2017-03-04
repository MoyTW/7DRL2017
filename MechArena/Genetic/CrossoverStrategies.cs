using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.Genetic
{
    public static class CrossoverStrategies
    {
        public static Individual<T> SinglePointCrossover<T>(Individual<T> parentA, Individual<T> parentB, Random rand)
        {
            Individual<T> child = new Individual<T>(parentA);

            int crossoverAt = rand.Next(parentA.ChromosomeSize);
            for (int i = crossoverAt; i < parentA.ChromosomeSize; i++)
            {
                child.SetGene(i, parentB.GetGene(i));
            }

            return child;
        }
    }
}
