/**
 * @Name RackSpaceLogic.cs
 * @Purpose 
 * @Date 07 October 2021, 11:37:25
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.BusinessLogic
{
	#region -- Using directives --
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Logging;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.EntityFrameworkCore;
	using System.Data.Common;
	using System.Data;
	using Oracle.ManagedDataAccess.Client;
	using PXS.IfRF.Supporting;
	using PXS.IfRF.Services;
	#endregion

	public class RackSpaceLogic : BusinessLogic
	{
		public RackSpaceLogic
		(
			ModelContext		ifrfContext
		,	ILoggerManager		logger
		) : base(ifrfContext, logger)
		{
		}

		public override BusinessLogicResult AfterCreate(SpecificResource specificResource)
		{
			return ( null);
		}

		public override BusinessLogicResult AfterDelete(SpecificResource resource)
		{
			return ( null);
		}

		public override BusinessLogicResult AfterUpdate(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return ( null);
		}

		public override BusinessLogicResult ValidateCreate( SpecificResource newEntity )
		{
			return ( null);
		}

		public override BusinessLogicResult ValidateDelete(SpecificResource resource)
		{
			return ( null);
		}

		public override BusinessLogicResult ValidateEdit(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return ( null);
		}
	}
}
