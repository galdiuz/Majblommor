using Majblommor.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majblommor
{
    class Step
    {
        public List<ICommand> Commands { get; private set; }

        public Step()
        {
            Commands = new List<ICommand>();
        }

        public void Undo()
        {
            foreach(ICommand c in Commands)
            {
                c.UnExecute();
            }
        }

        public void Redo()
        {
            foreach (ICommand c in Commands)
            {
                c.Execute();
            }
        }
    }
}
