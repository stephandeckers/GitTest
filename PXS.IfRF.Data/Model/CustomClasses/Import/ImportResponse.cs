/**
 * @Name ImportResponse.cs
 * @Purpose 
 * @Date 01 February 2022, 07:41:28
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model.Import
{
	#region -- Using directives --
	using System.Collections.Generic;
	#endregion

	public class ImportResponse
	{
		public List<string> Messages { get; set; }

        public ImportResponse()
        {
            Messages = new List<string>();
        }

        public ImportResponse(string errorMessage)
        {
            Messages = new List<string> { errorMessage };
        }
	}
}
