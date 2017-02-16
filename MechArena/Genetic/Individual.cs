using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class Individual<T>
    {
        private bool dirty;
        private int fitness;
        private List<T> genes;

        public Func<Individual<T>, int> FitnessFn { get; }
        public bool Dirty { get { return this.dirty; } }
        public int ChromosomeSize { get; }
        public int Fitness {
            get
            {
                if (dirty)
                {
                    this.fitness = this.FitnessFn(this);
                    this.dirty = false;
                }
                return this.fitness;
            }
        }

        public Individual(GeneListing<T> factory, int chromosomeSize, Func<Individual<T>, int> fitnessFn, Random rand)
        {
            this.dirty = true;
            this.FitnessFn = fitnessFn;
            this.ChromosomeSize = chromosomeSize;

            this.genes = new List<T>(chromosomeSize);
            for(int i = 0; i < chromosomeSize; i++)
            {
                this.genes.Add(factory.SelectRandomGene(rand));
            }
        }

        public Individual(Individual<T> parent)
        {
            if (parent.Dirty)
            {
                this.dirty = true;
            }
            else
            {
                this.dirty = false;
                this.fitness = parent.Fitness;
            }

            this.FitnessFn = parent.FitnessFn;
            this.ChromosomeSize = parent.ChromosomeSize;
            this.genes = new List<T>(parent.InspectGenes());
        }

        public IList<T> InspectGenes()
        {
            return this.genes.AsReadOnly();
        }

        public T GetGene(int idx)
        {
            return this.genes[idx];
        }

        public void SetGene(int idx, T gene)
        {
            this.genes[idx] = gene;
            this.dirty = true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var gene in this.genes)
            {
                builder.Append(gene.ToString());
            }
            return builder.ToString();
        }
    }
}
