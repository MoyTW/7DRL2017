using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena.Mech
{
    public class MechEntity
    {
        // Should be configurable!
        private static Dictionary<BodyPartLocations, int> MechTemplate = new Dictionary<BodyPartLocations, int>() {
            { BodyPartLocations.HEAD, 5 },
            { BodyPartLocations.TORSO, 15 },
            { BodyPartLocations.LEFT_ARM, 8 },
            { BodyPartLocations.RIGHT_ARM, 8 },
            { BodyPartLocations.LEFT_LEG, 10 },
            { BodyPartLocations.RIGHT_LEG, 10 }
        };
        private Dictionary<BodyPartLocations, BodyPart> bodyParts;

        public MechEntity()
        {
            this.bodyParts = new Dictionary<BodyPartLocations, BodyPart>();
            foreach (KeyValuePair<BodyPartLocations, int> kvp in MechTemplate)
            {
                this.bodyParts[kvp.Key] = new BodyPart(kvp.Key, kvp.Value);
            }
        }

        #region Inspection

        public int SlotsRemainingAt(BodyPartLocations location)
        {
            return bodyParts[location].SlotsRemaining;
        }

        public int SlotsUsedAt(BodyPartLocations location)
        {
            return bodyParts[location].SlotsUsed;
        }

        public BodyPart InspectPartAt(BodyPartLocations location)
        {
            return bodyParts[location];
        }

        public IList<Attachment> InspectAttachmentsAt(BodyPartLocations location)
        {
            return bodyParts[location].InspectAttachments();
        }

        #endregion

        #region State Changes

        public bool CanAttach(BodyPartLocations location, Attachment attachment)
        {
            return bodyParts[location].CanAttach(attachment);
        }

        public void Attach(BodyPartLocations location, Attachment attachment)
        {
            bodyParts[location].Attach(attachment);
        }

        public void Detach(BodyPartLocations location, Attachment attachment)
        {
            bodyParts[location].Detach(attachment);
        }

        #endregion
    }
}
