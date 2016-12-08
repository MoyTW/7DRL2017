using MechArena.Mech;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Mech
{
    [TestClass]
    public class BodyPartTest
    {
        private BodyPartLocations location = BodyPartLocations.HEAD;

        [TestMethod]
        public void TestTakeDamageBasic()
        {
            BodyPart part = new BodyPart(this.location, 10);
            for (int i = 0; i < 10; i++)
            {
                part.Attach(MechTestUtils.AttachmentWithSlots(1));
            }
            part.TakeDamage(3);
            Assert.AreEqual(7, part.InspectAttachments().Count);
        }

        [TestMethod]
        public void TestTakeDamageOverflow()
        {
            BodyPart part = new BodyPart(this.location, 10);
            for (int i = 0; i < 10; i++)
            {
                part.Attach(MechTestUtils.AttachmentWithSlots(1));
            }
            int overflow = part.TakeDamage(500);
            Assert.AreEqual(490, overflow);
        }

        // Probibalistic failures possible.
        [TestMethod]
        public void TestTakeDamageDistributed()
        {
            BodyPart part = new BodyPart(this.location, 10);
            for (int i = 0; i < 9; i++)
            {
                part.Attach(MechTestUtils.AttachmentWithSlots(1));
            }
            part.Attach(MechTestUtils.AttachmentWithSlots(1, 99999));
            part.TakeDamage(25);

            int numLeft = part.InspectAttachments().Count;
            Assert.IsTrue(numLeft == 9 || numLeft == 10);
        }
    }
}
