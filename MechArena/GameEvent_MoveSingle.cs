namespace MechArena
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
        public int CurrentTick { get; }
        public XDirection X { get; }
        public YDirection Y { get; }
        public Arena GameArena { get; }

        public GameEvent_MoveSingle(Entity mover, int currentTick, XDirection x, YDirection y, Arena gameArena)
            : base(mover)
        {
            this.CurrentTick = currentTick;
            this.X = x;
            this.Y = y;
            this.GameArena = gameArena;
        }

        public GameEvent_MoveSingle(Entity mover, int currentTick, int x, int y, Arena gameArena)
            : this(mover, currentTick, (XDirection)x, (YDirection)y, gameArena) { }
    }
}
