using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.ErrorHandling
{
    // custom exception class for throwing application specific exceptions 
    // that can be caught and handled within the application
    public class ServiceException : Exception
    {
        public ServiceException() : base() { }

        public ServiceException(string message) : base(message) 
        { 
        }

        public ServiceException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
