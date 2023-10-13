using System;
using System.Collections.Generic;
using System.Text;

namespace Othello
{
    public class ComputerPlayer_Intermediate : ComputerPlayer_Basic
    {
        public ComputerPlayer_Intermediate(bool amIWhite = true) : base(amIWhite)
        {
            LevelName = "Intermediate";
        }
    }
}
