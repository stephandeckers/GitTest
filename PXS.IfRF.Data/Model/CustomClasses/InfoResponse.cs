/**
* @Name InfoResponse.cs
* @Purpose 
* @Date 01 August 2021, 10:23:45
* @Author S.Deckers
* @Description 
*/

namespace PXS.IfRF.Data.Model
{
	using System.Collections.Generic;

	public class InfoResponse
	{
		public string AssemblyVersion	{ get; set; }
		public string BuildDate			{ get; set; }
		public string LastDdlTime		{ get; set; }
		public string DbVersion			{ get; set; }
		public string ConsoleLogLevel	{ get; set; }
		public string FileLogLevel		{ get; set; }

		public string UserId { get; set; }
        public IList<string> UserIvGroups { get; set; }
    }
}
