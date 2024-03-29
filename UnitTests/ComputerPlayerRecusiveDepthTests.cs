﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OthelloLib;
using System;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class ComputerPlayerRecusiveDepthTests
    {
        /// <summary>
        /// Expert plays full games vs Advanced trying all odd Expert Depths [3-29]
        /// L  3= .2"
        /// W  5= .4"
        /// L  7= .6"
        /// W  9= .7"
        /// W 11= .7"
        /// L 13= .8"
        /// W 15= .8"
        /// W 17= .8"
        /// W 19= .9"
        /// W 21= .9"
        /// W 23=1.0"
        /// L 25=1.0"
        /// T 27=1.1"
        /// W 29=1.4"
        /// </summary>
        [TestMethod]
        public void TestExpertDepths()
        {
            // turn off LogDecisions for Computer vs Computer when doing multiple Runs so the Scores can be easily seen
            bool prevLogDecisions = ComputerPlayer.LogDecisions; // cache to reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = false; // comment this out to see Decisions for Computer vs Computer

            ComputerPlayer advanced = new ComputerPlayer_Advanced(true);

            for (int depth = 3; depth <= 29; depth+=2)
            {
                ComputerPlayer expert = new ComputerPlayer_Expert(false, depth);
                RunAndLog(expert, advanced);
            }

            ComputerPlayer.LogDecisions = prevLogDecisions;
        }

        /// <summary>
        /// Ultimate plays full games vs Advanced trying all odd Ultimate Depths 5+[2-8]=[7-13] reverting to Expert at -5=[2-8]
        /// L 2, 7=  0.6"
        /// W 4, 9=  3"
        /// W 6,11= 36"
        /// W 8,13=640"
        /// </summary>
        [TestMethod]
        public void TestUltimateDepths()
        {
            // turn off LogDecisions for Computer vs Computer when doing multiple Runs so the Scores can be easily seen
            bool prevLogDecisions = ComputerPlayer.LogDecisions; // cache to reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = false; // comment this out to see Decisions for Computer vs Computer

            ComputerPlayer advanced = new ComputerPlayer_Advanced(true);

            for (int depthForEveryMove = 2; depthForEveryMove <= 6; depthForEveryMove+=2)
            {
                ComputerPlayer ultimate = new ComputerPlayer_Ultimate(false, depthForEveryMove, depthForEveryMove + 5);
                RunAndLog(ultimate, advanced);
            }

            ComputerPlayer.LogDecisions = prevLogDecisions;
        }

        /// <summary>
        /// Ultimate plays full games vs Advanced trying Ultimate Depth=11 reverting to Expert at [3-9]
        /// W 3,11=  1"
        /// W 5,11=  7"
        /// W 7,11= 38"
        /// L 9,11=325" 
        /// </summary>
        [TestMethod]
        public void TestUltimateDepthSwitching()
        {
            // turn off LogDecisions for Computer vs Computer when doing multiple Runs so the Scores can be easily seen
            bool prevLogDecisions = ComputerPlayer.LogDecisions; // cache to reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = false; // comment this out to see Decisions for Computer vs Computer

            ComputerPlayer advanced = new ComputerPlayer_Advanced(true);

            for (int depthForEveryMove = 3; depthForEveryMove <= 7; depthForEveryMove+=2)
            {
                ComputerPlayer ultimate = new ComputerPlayer_Ultimate(false, depthForEveryMove, 11);
                RunAndLog(ultimate, advanced);
            }

            ComputerPlayer.LogDecisions = prevLogDecisions;
        }

        private void RunAndLog(ComputerPlayer black, ComputerPlayer white)
        {
            Debug.Print(black.ToString() + "  vs.  " + white.ToString());
            DateTime start = DateTime.Now;
            BoardState boardState = OthelloLib.ComputerPlayers.ComputerVsComputer(black, white);
            TimeSpan duration = DateTime.Now - start;
            int diff = boardState.BlackCount - boardState.WhiteCount;
            Debug.Print("Black's lead = {0} in {1:N2} seconds\n", diff, duration.TotalSeconds);
        }
    }
}
