using System.Collections.Generic;

namespace PXS.IfRF.BusinessLogic
{
    /// <summary>
    /// Contains the results of a validation or applying a business logic.
    /// The logic is considered as successful if there are no error messages
    /// </summary>
    public class BusinessLogicResult
    {
        public BusinessLogicResult()
        {
            ErrorMessages = new List<string>();
        }

        public BusinessLogicResult(string errorMessage)
        {
            ErrorMessages = new List<string> { errorMessage };
        }

        public bool Succeeded
        {
            get
            {
                return ErrorMessages.Count == 0;
            }
        }

        public List<string> ErrorMessages { get; set; }
    }
}
