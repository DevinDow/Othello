using Othello;
using OthelloLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerVsComputer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerPlayer black = new ComputerPlayer_Advanced(false);
            ComputerPlayer white = new ComputerPlayer_Ultimate(true);

            ComputerPlayers.ComputerVsComputer(black, white);
        }
    }
}
