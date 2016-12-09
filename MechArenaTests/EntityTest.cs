using MechArena;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests
{
    [TestClass]
    public class EntityTest
    {
        private Entity ent = new Entity(Guid.NewGuid());
        private Component cmp = new Component();

        [TestMethod]
        public void TestAddComponent()
        {
            this.ent.AddComponent(this.cmp);
            Assert.AreEqual(1, this.ent.InspectComponents().Count);
            Assert.AreEqual(this.cmp, this.ent.InspectComponents()[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddComponentDouble()
        {
            this.ent.AddComponent(this.cmp);
            this.ent.AddComponent(this.cmp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveComponentThrows()
        {
            this.ent.RemoveComponent(this.cmp);
        }

        [TestMethod]
        public void TestRemoveComponent()
        {
            var a = new List<String>();
            this.ent.AddComponent(this.cmp);
            this.ent.RemoveComponent(this.cmp);
        }
    }
}
