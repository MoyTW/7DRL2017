using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class GeneListing<T>
    {
        private List<T> possibleValues;

        public GeneListing(IEnumerable<T> possibleValues)
        {
            this.possibleValues = new List<T>(possibleValues);
        }

        public T SelectRandomGene(Random rand)
        {
            return this.possibleValues[rand.Next(this.possibleValues.Count)];
        }
    }
}
