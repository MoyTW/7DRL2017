using MechArena.Mech;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Mech
{
    [TestClass]
    public class MechEntityTests
    {
        [TestMethod]
        public void ConstructorBuildsAllBodyPartsTest()
        {
            var mech = new MechEntity();

            foreach(BodyPartLocations location in Enum.GetValues(typeof(BodyPartLocations)))
            {
                Assert.IsInstanceOfType(mech.InspectPartAt(location), typeof(BodyPart));
            }
        }

        [TestMethod]
        public void CanAttach()
        {
            var mech = new MechEntity();

            Attachment a = MechTestUtils.AttachmentWithSlots(1);
            mech.Attach(BodyPartLocations.HEAD, a);
            Assert.AreSame(a, mech.InspectAttachmentsAt(BodyPartLocations.HEAD)[0]);
            Assert.AreEqual(1, mech.SlotsUsedAt(BodyPartLocations.HEAD));
            Assert.AreEqual(4, mech.SlotsRemainingAt(BodyPartLocations.HEAD));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotAttachHugeItem()
        {
            var mech = new MechEntity();

            Attachment a = MechTestUtils.AttachmentWithSlots(9999);

            mech.Attach(BodyPartLocations.HEAD, a);
        }

        [TestMethod]
        public void CanDetach()
        {
            var mech = new MechEntity();

            Attachment a = MechTestUtils.AttachmentWithSlots(1);

            mech.Attach(BodyPartLocations.HEAD, a);
            mech.Detach(BodyPartLocations.HEAD, a);
            Assert.AreEqual(0, mech.InspectAttachmentsAt(BodyPartLocations.HEAD).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotDetachPhantomAttachment()
        {
            var mech = new MechEntity();

            Attachment a = MechTestUtils.AttachmentWithSlots(1);

            mech.Detach(BodyPartLocations.HEAD, a);
        }
    }
}
