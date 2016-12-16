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

    public class GameEvent_MoveSingle : GameEvent
    {
        public int CurrentTick { get; }
        public XDirection X { get; }
        public YDirection Y { get; }

        public GameEvent_MoveSingle(int currentTick, XDirection x, YDirection y)
        {
            this.CurrentTick = currentTick;
            this.X = x;
            this.Y = y;
        }
    }
}
