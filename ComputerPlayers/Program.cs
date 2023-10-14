using Othello;
using OthelloLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ComputerVsComputer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerPlayers.OutputToConsole = true;

            ComputerPlayer black = new ComputerPlayer_Advanced(false);
            ComputerPlayer white = new ComputerPlayer_Ultimate(true);

            Console.WriteLine("{0}  vs.  {1}", black, white);
            BoardState boardState = ComputerPlayers.ComputerVsComputer(black, white);
            Console.WriteLine(boardState.ToString());
            Console.WriteLine("Black={0} White={1}", boardState.BlackCount, boardState.WhiteCount);
            Console.WriteLine("{0}  vs.  {1}", black, white);
            Console.ReadKey();
        }
    }
}
