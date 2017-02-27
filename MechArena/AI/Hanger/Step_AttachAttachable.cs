using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena.AI.Hanger
{
	public class Step_AttachAttachable : Step
	{
        public string BlueprintLabel { get; }
        public BodyPartLocation Location { get; }

        public Step_AttachAttachable() { }

        public Step_AttachAttachable(string blueprintLabel, BodyPartLocation location)
        {
            this.BlueprintLabel = blueprintLabel;
            this.Location = location;
        }

        public override void ApplyStep (Entity entity)
		{
            var entityToAttach = BlueprintListing.BuildForLabel(this.BlueprintLabel);
            var matchingAttachPoint = entity.GetComponentOfType<Component_MechSkeleton>()
                .InspectBodyPart(this.Location)
                .HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.ATTACH_POINT))
                .SubEntities
                .Where(e => e.GetComponentOfType<Component_AttachPoint>().CanAttach(entityToAttach))
                .FirstOrDefault();
            if (matchingAttachPoint != null)
            {
                // TODO: WHY DOESN'T THE EVENT PROPOGATE FROM THE TOP-LEVEL ENTITY
                // OH GOD I SPENT A WHOLE DAY ON THIS
                matchingAttachPoint.HandleEvent(new GameEvent_Slot(entity, matchingAttachPoint, entityToAttach));
            }
        }

		public override IEnumerable<SingleClause> EnumerateClauses ()
		{
            var attachableLabels = BlueprintListing.GetMatchingBlueprints(SubEntitiesSelector.ATTACHABLE)
                .Select(b => b.Label);
            foreach (var label in attachableLabels)
            {
                foreach (var location in Component_MechSkeleton.TemplateLocations)
                {
                    yield return new Step_AttachAttachable(label, location);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("[Step_AttachAttachable: BlueprintLabel={0}, Location={1}]", BlueprintLabel, Location);
        }
    }
}

