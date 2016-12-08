using MechArena.Mech;

namespace MechArenaTests.Mech
{
    static class MechTestUtils
    {
        public static Attachment AttachmentWithSlots(int slots, int size=1)
        {
            return new Attachment("", slots, size, null);
        }
    }
}
