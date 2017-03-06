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

    public class GameEvent_MoveSingle : GameEvent_Command
    {
        public XDirection X { get; }
        public YDirection Y { get; }
        public ArenaState GameArena { get; }

        public GameEvent_MoveSingle(int commandTick, int APCost, Entity mover, XDirection x, YDirection y, ArenaState gameArena)
            : base(commandTick, APCost, mover)
        {
            this.X = x;
            this.Y = y;
            this.GameArena = gameArena;
        }

        public GameEvent_MoveSingle(int commandTick, int APCost, Entity mover, int x, int y, ArenaState gameArena)
            : this(commandTick, APCost, mover, (XDirection)x, (YDirection)y, gameArena) { }
    }
}
