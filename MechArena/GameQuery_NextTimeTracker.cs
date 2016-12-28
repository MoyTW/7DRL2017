using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class GameQuery_NextTimeTracker : GameQuery
    {
        private int currentTick;
        private Entity nextEntity;
        private int nextEntityTicksToLive;

        public Entity NextEntity { get { return nextEntity; } }
        public int NextEntityTicksToLive { get { return nextEntityTicksToLive; } }

        public GameQuery_NextTimeTracker(int currentTick)
        {
            this.currentTick = currentTick;
            this.nextEntityTicksToLive = Int32.MaxValue;
        }

        public void RegisterEntity(Entity registeredEntity)
        {
            var registeredTicksTolive = registeredEntity.TryGetTicksToLive(this.currentTick);

            // Note that CommandEntities are always first in a tie - hence only override on greater than!
            if (registeredTicksTolive < this.nextEntityTicksToLive)
            {
                this.nextEntity = registeredEntity;
                this.nextEntityTicksToLive = registeredTicksTolive;
            }
        }
    }
}
