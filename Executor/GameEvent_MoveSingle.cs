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
        public Entity Mover { get; }
        public XDirection X { get; }
        public YDirection Y { get; }

        public CommandStub_MoveSingle(Entity mover, int x, int y)
        {
            this.Mover = mover;
            this.X = (XDirection)x;
            this.Y = (YDirection)y;
        }
    }

    public class GameEvent_MoveSingle : GameEvent_Command
    {
        public XDirection X { get; }
        public YDirection Y { get; }
        public ArenaState GameArena { get; }

        private GameEvent_MoveSingle(int commandTick, int APCost, Entity mover, XDirection x, YDirection y, ArenaState gameArena)
            : base(commandTick, APCost, mover)
        {
            this.X = x;
            this.Y = y;
            this.GameArena = gameArena;
        }

        public static GameEvent_MoveSingle ResolveStub(CommandStub_MoveSingle stub, ArenaState arena)
        {
            return new GameEvent_MoveSingle(arena.CurrentTick, Config.ONE, stub.Mover, stub.X, stub.Y, arena);
        }
    }
}
