using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;
using System;

namespace UnitTests
{
    [TestClass]
    public class CoordTests
    {
        [TestMethod]
        public void TestEquatable()
        {
            Coord coord1 = new Coord(4, 5);
            Coord coord2 = new Coord(4, 5);
            Coord coord3 = new Coord(3, 5);
            Coord coord4 = new Coord(4, 6);

            Assert.AreEqual(coord1, coord2);
            Assert.AreNotEqual(coord1, coord3);
            Assert.AreNotEqual(coord1, coord4);
        }
    }
}
