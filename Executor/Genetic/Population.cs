using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.Genetic
{
    public class Population<T>
    {
        private List<Individual<T>> population;

        public int CurrentSize { get { return this.population.Count; } }
        public int DesiredSize { get; }

        public Population(int desiredSize)
        {
            this.population = new List<Individual<T>>(desiredSize);
            this.DesiredSize = desiredSize;
        }

        public void AddIndividual(Individual<T> newIndividual)
        {
            if (this.CurrentSize < this.DesiredSize)
                this.population.Add(newIndividual);
            else
                throw new InvalidOperationException("Can't add individuals to already full population!");
        }

        public void AddIndividual(Individual<T> newIndividual, int idx)
        {
            this.population[idx] = newIndividual;
        }

        public void RemoveIndividualAt(int idx)
        {
            this.population.RemoveAt(idx);
        }

        public Individual<T> GetIndividual(int idx)
        {
            return this.population[idx];
        }

        public IList<Individual<T>> InspectIndividuals()
        {
            return this.population.AsReadOnly();
        }

        public int HighestFitness()
        {
            if (this.CurrentSize > 0)
                return this.population.Max(i => i.Fitness);
            else
                return -1;
        }

        public Individual<T> HighestFitnessIndividual()
        {
            return this.population.OrderByDescending(i => i.Fitness).FirstOrDefault();
        }
    }
}
