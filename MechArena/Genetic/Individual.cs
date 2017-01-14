using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class Individual<T>
    {
        public int ChromosomeSize { get; }
        private List<T> genes;

        public Individual(GeneFactory<T> factory, int chromosomeSize, Random rand)
        {
            this.ChromosomeSize = chromosomeSize;

            this.genes = new List<T>(chromosomeSize);
            for(int i = 0; i < chromosomeSize; i++)
            {
                this.genes.Add(factory.SelectRandomGene(rand));
            }
        }

        public Individual(Individual<T> parent)
        {
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
