/**
 * @Name Availability.cs
 * @Purpose 
 * @Date 21 July 2021, 10:56:45
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Microsoft.EntityFrameworkCore;
	#endregion

	public class AvailabilityRequest
	{
		/// <summary>
		/// Gets or sets the name of the pop.
		/// </summary>
		/// <value>
		/// The name of the pop.
		/// </value>
		[ Required( )]
		public string	PopName			{ get; set; }

		/// <summary>
		/// Gets or sets the OwnerKey.
		/// </summary>
		/// <value>
		/// The OwnerKey.
		/// </value>
		[ Required( )]
		public string	OwnerKey		{ get; set; }

		public int?		OdfTrayNr		{ get; set; }

		/// <summary>
		/// Gets or sets the operational status.
		/// </summary>
		/// <value>
		/// The operational status, if not set 'O' will be taken
		/// </value>
		public string	OperationalStatus	{ get; set; }

		/// <summary>
		/// Gets or sets the type of the olt position.
		/// </summary>
		/// <value>
		/// Defaults to 'GPON' if not set
		/// </value>
		public string	OltPositionType	{ get; set; }
	}

	public class AvailabilityResponse
	{
		public SrPosition	SplitterPosition	{ get; set; }
		public SrPosition	OLTPosition			{ get; set; }
		public Connection	Connection			{ get; set; }
		public List<string> Errors				{ get; set; }
	}
}
