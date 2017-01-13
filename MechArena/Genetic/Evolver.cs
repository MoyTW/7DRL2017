using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class Evolver
    {
        private bool keepHistory;
        private List<Population> history;
        private Random rand;
        private int currentGeneration, requiredFitness, maxGenerations;
        private double mutationRate;
        private Population currentPopulation;

        public Evolver(int requiredFitness, int maxGenerations, double mutationRate, int populationSize, IGene gene,
            int chromosomeSize, int seed=1, bool keepHistory = true)
        {
            this.rand = new Random(seed);
            this.keepHistory = keepHistory;
            if (this.keepHistory)
            {
                this.history = new List<Population>();
            }
            this.currentGeneration = 0;
            this.requiredFitness = requiredFitness;
            this.maxGenerations = maxGenerations;
            this.mutationRate = mutationRate;
            this.currentPopulation = new Population(populationSize);

            // Randomly build the first generation
            for (int i = 0; i < populationSize; i++)
            {
                this.currentPopulation.AddIndividual(new Individual(gene, chromosomeSize, rand));
            }
        }

        public Individual Evolve(Func<Individual, int> fitness, Func<Population, Random, Individual> selectParent,
            Func<Individual, Individual, Random, Individual> crossover, Action<Individual, Random> mutate,
            Func<Individual, bool> isSurvivor)
        {
            // silly, cache the fitness
            while (this.currentPopulation.HighestFitness(fitness) < this.requiredFitness &&
                this.currentGeneration < this.maxGenerations)
            {
                this.AdvanceGeneration(selectParent, crossover, mutate, isSurvivor);
            }

            return this.currentPopulation.HighestFitnessIndividual(fitness);
        }

        public void AdvanceGeneration(Func<Population, Random, Individual> selectParent, Func<Individual, Individual, Random, Individual> crossover,
            Action<Individual, Random> mutate, Func<Individual, bool> isSurvivor)
        {
            // Breed next generation (fully replaces current generation)
            Population newPopulation = new Population(this.currentPopulation.DesiredSize);
            for (int i = 0; i < currentPopulation.DesiredSize; i++)
            {
                Individual parentOne = selectParent(this.currentPopulation, this.rand);
                Individual parentTwo = selectParent(this.currentPopulation, this.rand);
                Individual offspring = crossover(parentOne, parentTwo, this.rand);
                newPopulation.AddIndividual(offspring);
            }
            if (this.keepHistory)
                this.history.Add(this.currentPopulation);
            this.currentPopulation = newPopulation;

            // Mutate next generation
            for (int i = 0; i < currentPopulation.CurrentSize; i++)
            {
                if (this.rand.NextDouble() <= this.mutationRate)
                {
                    mutate(this.currentPopulation.GetIndividual(i), this.rand);
                }
            }

            // Cull next generation
            for (int i = this.currentPopulation.CurrentSize - 1; i >= 0; i--)
            {
                if (!isSurvivor(this.currentPopulation.GetIndividual(i)))
                    this.currentPopulation.RemoveIndividualAt(i);
            }

            this.currentGeneration++;
        }

        public IList<Population> InspectHistory()
        {
            return this.history.AsReadOnly();
        }
    }
}
