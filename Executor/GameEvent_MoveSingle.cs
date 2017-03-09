namespace Executor
{
    public enum XDirection
    {
        LEFT = -1,
        RIGHT = 1,
        NONE = 0
    }

    public enum YDirection
    {
        UP = -1,
        DOWN = 1,
        NONE = 0
    }

    public class CommandStub_MoveSingle : CommandStub
    {
        public XDirection X { get; }
        public YDirection Y { get; }

        public CommandStub_MoveSingle(string moverEID, int x, int y) : base(moverEID)
        {
            this.X = (XDirection)x;
            this.Y = (YDirection)y;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_MoveSingle(arena.CurrentTick, Config.ONE, arena.ResolveEID(this.CommandEID), this.X,
                this.Y, arena);
        }

        public override string ToString()
        {
            return string.Format("Move {0}, {1}", X, Y);
        }
    }

    public class GameEvent_MoveSingle : GameEvent_Command
    {
        public XDirection X { get; }
        public YDirection Y { get; }
        public ArenaState GameArena { get; }

        public override bool ShouldLog { get { return false; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0} moved [{1}, {2}]", this.CommandEntity.Label, this.X, this.Y);
            }
        }

        public GameEvent_MoveSingle(int commandTick, int APCost, Entity mover, XDirection x, YDirection y, ArenaState gameArena)
            : base(commandTick, APCost, mover)
        {
            this.X = x;
            this.Y = y;
            this.GameArena = gameArena;
        }
    }
}
