using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena.AI.Hanger
{
    public class Step_SlotPart : Step
    {
        public string BlueprintLabel { get; }
        public BodyPartLocation Location { get; }

        public Step_SlotPart() { }

        public Step_SlotPart(string blueprintLabel, BodyPartLocation location)
        {
            this.BlueprintLabel = blueprintLabel;
            this.Location = location;
        }

        public override void ApplyStep(Entity entity)
        {
            var slottable = BlueprintListing.BuildForLabel(this.BlueprintLabel);
            var container = entity.GetComponentOfType<Component_MechSkeleton>().InspectBodyPart(this.Location);
            entity.HandleEvent(new GameEvent_Slot(entity, container, slottable));
        }

        public override IEnumerable<SingleClause> EnumerateClauses()
        {
            var slottableLabels = BlueprintListing.GetMatchingBlueprints(SubEntitiesSelector.SLOTTABLE)
                .Select(b => b.Label);
            foreach (var label in slottableLabels)
            {
                foreach (var location in Component_MechSkeleton.TemplateLocations)
                {
                    yield return new Step_SlotPart(label, location);
                }
            }
        }
    }
}

