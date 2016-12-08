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
    }
}
