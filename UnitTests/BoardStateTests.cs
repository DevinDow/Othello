using Microsoft.VisualStudio.TestTools.UnitTesting;
using OthelloLib;
using System;

namespace UnitTests
{
    [TestClass]
    public class BoardStateTests
    {
        [TestMethod]
        public void TestInitialState()
        {
            BoardState state = new BoardState();
            Assert.AreEqual(StateEnum.Black, state.GetSquare(new Coord(4, 4)).State);
            Assert.AreEqual(StateEnum.Black, state.GetSquare(new Coord(5, 5)).State);
            Assert.AreEqual(StateEnum.White, state.GetSquare(new Coord(4, 5)).State);
            Assert.AreEqual(StateEnum.White, state.GetSquare(new Coord(5, 4)).State);

            Assert.AreEqual(StateEnum.Empty, state.GetSquare(new Coord(1, 1)).State);
        }
    }
}
