using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class Evolver<T>
    {
        private bool keepHistory;
        private List<Population<T>> history;
        private Random rand;
        private int currentGeneration, requiredFitness, maxGenerations;
        private double mutationRate;
        private Population<T> currentPopulation;

        public int CurrentGeneration { get { return this.currentGeneration; } }

        public Evolver(int requiredFitness, Func<Individual<T>, int> fitnessFn, int maxGenerations,
            double mutationRate, int populationSize, GeneListing<T> factory, int chromosomeSize, int seed=1,
            bool keepHistory = true)
        {
            this.rand = new Random(seed);
            this.keepHistory = keepHistory;
            if (this.keepHistory)
            {
                this.history = new List<Population<T>>();
            }
            this.currentGeneration = 0;
            this.requiredFitness = requiredFitness;
            this.maxGenerations = maxGenerations;
            this.mutationRate = mutationRate;
            this.currentPopulation = new Population<T>(populationSize);

            // Randomly build the first generation
            for (int i = 0; i < populationSize; i++)
            {
                this.currentPopulation.AddIndividual(new Individual<T>(factory, chromosomeSize, fitnessFn, rand));
            }
        }

        public Population<T> InspectCurrentPopulation() { return this.currentPopulation; }

        public Individual<T> Evolve(Func<Population<T>, Random, Individual<T>> selectParent,
            Func<Individual<T>, Individual<T>, Random, Individual<T>> crossover, Action<Individual<T>, Random> mutate,
            Func<Population<T>, Individual<T>, bool> isSurvivor)
        {
            // silly, cache the fitness
            while (this.currentPopulation.HighestFitness() < this.requiredFitness &&
                this.currentGeneration < this.maxGenerations)
            {
                this.AdvanceGeneration(selectParent, crossover, mutate, isSurvivor);
            }

            return this.currentPopulation.HighestFitnessIndividual();
        }

        public void AdvanceGeneration(Func<Population<T>, Random, Individual<T>> selectParent,
            Func<Individual<T>, Individual<T>, Random, Individual<T>> crossover, Action<Individual<T>, Random> mutate,
            Func<Population<T>, Individual<T>, bool> isSurvivor)
        {
            Console.WriteLine("Advancing generation! Population size: " + this.currentPopulation.CurrentSize);

            // Breed next generation (fully replaces current generation)
            Population<T> newPopulation = new Population<T>(this.currentPopulation.DesiredSize);
            for (int i = 0; i < currentPopulation.DesiredSize; i++)
            {
                Individual<T> parentOne = selectParent(this.currentPopulation, this.rand);
                Individual<T> parentTwo = selectParent(this.currentPopulation, this.rand);
                Individual<T> offspring = crossover(parentOne, parentTwo, this.rand);
                newPopulation.AddIndividual(offspring);
            }
            if (this.keepHistory)
                this.history.Add(this.currentPopulation);
            if (newPopulation.CurrentSize != newPopulation.DesiredSize)
                throw new InvalidOperationException("Can't proceed, population did not breed!");
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
                if (!isSurvivor(this.currentPopulation, this.currentPopulation.GetIndividual(i)))
                    this.currentPopulation.RemoveIndividualAt(i);
            }

            this.currentGeneration++;
        }

        public IList<Population<T>> InspectHistory()
        {
            return this.history.AsReadOnly();
        }
    }
}
