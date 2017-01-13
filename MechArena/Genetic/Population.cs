using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class Population
    {
        private List<Individual> population;

        public int CurrentSize { get { return this.population.Count; } }
        public int DesiredSize { get; }

        public Population(int desiredSize)
        {
            this.population = new List<Individual>(desiredSize);
            this.DesiredSize = desiredSize;
        }

        public void AddIndividual(Individual newIndividual)
        {
            if (this.CurrentSize < this.DesiredSize)
                this.population.Add(newIndividual);
            else
                throw new InvalidOperationException("Can't add individuals to already full population!");
        }

        public void AddIndividual(Individual newIndividual, int idx)
        {
            this.population[idx] = newIndividual;
        }

        public void RemoveIndividualAt(int idx)
        {
            this.population.RemoveAt(idx);
        }

        public Individual GetIndividual(int idx)
        {
            return this.population[idx];
        }

        public IList<Individual> InspectIndividuals()
        {
            return this.population.AsReadOnly();
        }

        public int HighestFitness(Func<Individual, int> fitness)
        {
            if (this.CurrentSize > 0)
                return this.population.Max(fitness);
            else
                return -1;
        }

        public Individual HighestFitnessIndividual(Func<Individual, int> fitness)
        {
            return this.population.OrderByDescending(fitness).FirstOrDefault();
        }
    }
}
