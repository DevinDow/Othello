using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloLib
{
    public class ComputerPlayer_Intermediate : ComputerPlayer_Basic
    {
        public ComputerPlayer_Intermediate(bool amIWhite = true) : base(amIWhite)
        {
            LevelName = "Intermediate";
        }
    }
}
