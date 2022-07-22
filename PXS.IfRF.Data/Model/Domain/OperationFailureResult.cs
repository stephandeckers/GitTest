using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PXS.IfRF.Data.Model
{
    public class OperationFailureResult
    {
        public OperationFailureResult()
        {
            ErrorMessages = new List<string>();
        }

        public OperationFailureResult(List<string> errorMessages)
        {
            ErrorMessages = errorMessages;
        }

        public OperationFailureResult(string errorMessage)
        {
            ErrorMessages = new List<string> { errorMessage };
        }

        [NotMapped]
        public List<string> ErrorMessages { get; set; }


    }
}
