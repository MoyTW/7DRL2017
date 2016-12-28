using RogueSharp.Random;

namespace MechArena
{
    public class GameQuery_Command : GameQuery
    {
        private GameEvent_Command command;

        public Entity CommandEntity { get; }
        public Entity ExecutorEntity { get; }
        public Arena ArenaState { get; }
        public IRandom Rand { get { return this.ArenaState.SeededRand; } }

        public GameEvent_Command Command { get { return this.command; } }

        public GameQuery_Command(Entity commandEntity, Entity executorEntity, Arena arenaState)
        {
            this.CommandEntity = commandEntity;
            this.ExecutorEntity = executorEntity;
            this.ArenaState = arenaState;
        }

        public void RegisterCommand(GameEvent_Command command)
        {
            this.command = command;
            this.Completed = true;
        }
    }
}
