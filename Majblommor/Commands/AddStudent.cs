using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majblommor.Commands
{
    class AddStudent : ICommand
    {
        private SchoolClass C;
        private Student S;

        public AddStudent(SchoolClass c, Student s)
        {
            C = c;
            S = s;
        }

        public void Execute()
        {
            C.Students.Add(S);
        }

        public void UnExecute()
        {
            C.Students.Remove(S);
        }
    }
}
