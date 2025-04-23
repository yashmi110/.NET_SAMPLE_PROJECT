using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Exceptions
{
    public class InvalidInputException : Exception
    {
        public string FieldName { get; }
        public object InvalidValue { get; }

        public InvalidInputException(string fieldName, object invalidValue)
            : base($"Invalid value '{invalidValue}' for field '{fieldName}'")
        {
            FieldName = fieldName;
            InvalidValue = invalidValue;
        }
    }

}
