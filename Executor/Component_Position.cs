using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    class Component_Position : Component
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool BlocksMovement { get; private set; }
        public bool BlocksMovementWhenDestroyed { get; }

        public Component_Position(int x, int y, bool blocksMovement, bool blocksMovementWhenDestroyed=false)
        {
            this.X = x;
            this.Y = y;
            this.BlocksMovement = blocksMovement;
            this.BlocksMovementWhenDestroyed = blocksMovementWhenDestroyed;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleMove(GameEvent_MoveSingle ev)
        {
            if (ev.GameArena.IsWalkableAndOpen(this.X + (int)ev.X, this.Y + (int)ev.Y))
            {
                this.X += (int)ev.X;
                this.Y += (int)ev.Y;
            }
            ev.Completed = true;
        }

        private void HandleDestroy(GameEvent_Destroy ev)
        {
            this.BlocksMovement = this.BlocksMovementWhenDestroyed;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_MoveSingle)
                this.HandleMove((GameEvent_MoveSingle)ev);
            if (ev is GameEvent_Destroy)
                this.HandleDestroy((GameEvent_Destroy)ev);

            return ev;
        }

        private void HandleQueryPosition(GameQuery_Position q)
        {
            q.RegisterPosition(this.X, this.Y, this.BlocksMovement);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Position)
                this.HandleQueryPosition((GameQuery_Position)q);

            return q;
        }
    }
}
