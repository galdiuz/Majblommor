using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majblommor.Commands
{
    class RemoveStudent : ICommand
    {
        private SchoolClass C;
        private Student S;

        public RemoveStudent(SchoolClass c, Student s)
        {
            C = c;
            S = s;
        }

        public void Execute()
        {
            C.Students.Remove(S);
        }

        public void UnExecute()
        {
            C.Students.Add(S);
        }
    }
}
