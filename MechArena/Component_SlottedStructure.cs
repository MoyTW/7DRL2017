using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    // Depends on Component_SlottedContainer! Find some way to codify this in your EC system!
    // Assumes that slotted items have Componenet_InternalStructure! Ugh, TERRIBLE!
    public class Component_SlottedStructure : Component
    {
        private int GetRemainingInternalStructure(Entity e)
        {
            return e.GetComponentOfType<Component_InternalStructure>().StructureRemaining;
        }

        private int GetRemainingSlottedStructure(Component_SlottedContainer slottedContainer)
        {
            return slottedContainer.InspectStoredEntities()
                .Sum(e => this.GetRemainingInternalStructure(e));
        }

        private void AssignDamagePoint(Component_SlottedContainer slottedContainer)
        {
            var target = GameRandom.RandomByWeight<Entity>(slottedContainer.InspectStoredEntities(),
                (e => this.GetRemainingInternalStructure(e)));

            var damageEvent = new GameEvent_TakeDamage(1);
            target.HandleEvent(damageEvent);

            if (!damageEvent.Completed)
                throw new ArgumentException("Target didn't take damage when it should have!");

            if (target.GetComponentOfType<Component_InternalStructure>().Destroyed)
            {
                var unslotEvent = new GameEvent_Unslot(target, this.Parent);
                slottedContainer.Parent.HandleEvent(unslotEvent);
                if (!unslotEvent.Completed)
                    throw new ArgumentException("Target was destroyed but couldn't unslot from container!");
            }
        }

        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            var slottedContainer = this.Parent.GetComponentOfType<Component_SlottedContainer>();
            if (slottedContainer == null) { return; }

            int remainingSlottedStructure = GetRemainingSlottedStructure(slottedContainer);

            if (remainingSlottedStructure == 0)
            {
                return;
            }
            else if (ev.DamageRemaining >= remainingSlottedStructure)
            {
                ev.Notify_DamageTaken(remainingSlottedStructure);
                for (int i = 0; i < remainingSlottedStructure; i++)
                {
                    this.AssignDamagePoint(slottedContainer);
                }
            }
            else
            {
                int damageToTake = ev.DamageRemaining;
                for (int i = 0; i < damageToTake; i++)
                {
                    this.AssignDamagePoint(slottedContainer);
                }
                ev.Notify_DamageTaken(damageToTake);
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);

            return ev;
        }
    }
}
