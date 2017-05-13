using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Majblommor.Commands
{
    class ChangeValue : ICommand
    {
        private readonly object _object;
        private readonly PropertyInfo _property;
        private readonly dynamic _oldVal;
        private readonly dynamic _newVal;
        
        public ChangeValue(object obj, dynamic oldVal, dynamic newVal, [CallerMemberName] string propertyName = "")
        {
            _object = obj;
            _property = obj.GetType().GetProperty(propertyName);
            _oldVal = oldVal;
            _newVal = newVal;
        }
        
        public void Execute()
        {
            _property.SetValue(_object, _newVal);
        }

        public void UnExecute()
        {
            _property.SetValue(_object, _oldVal);
        }
    }
}
