using Executor.Tournament;
using RogueSharp.Random;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    class TournamentMapPicker : IMapPicker
    {
        private IRandom rand;

        public int NumMaps { get; }

        public TournamentMapPicker(int numMaps, IRandom rand)
        {
            this.NumMaps = numMaps;
            this.rand = rand;
        }

        public string PickMapID()
        {
            return rand.Next(NumMaps - 1).ToString();
        }
    }
}
