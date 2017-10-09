using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvFMML.Data.MML
{
    public class Bar
    {
        public LinkedList<Command.Command> CommandList { get; }
        public int Number { get; }
        public string SeperateSign { get; }

        public Bar(LinkedList<Command.Command> commandList, int number, string seperateSign)
        {
            CommandList = commandList;
            Number = number;
            SeperateSign = seperateSign;
        }

    }
}
