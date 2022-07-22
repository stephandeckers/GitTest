using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Text;
	using System.Collections.Generic;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNet.OData;
	using Oracle.ManagedDataAccess.Client;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	using PXS.IfRF.Logging;
	using d = System.Diagnostics.Debug;
	using PXS.IfRF.Data;
    using PXS.IfRF.BusinessLogic;
    #endregion

    public class SrPositionAggregateService : ISrPositionAggregateService
	{
		private readonly ModelContext _ifrfContext = null;
		private readonly ILoggerManager _logger = null;
		private readonly ISharedMethods _sharedMethods;

		public SrPositionAggregateService(ModelContext ifrfContext, ISharedMethods sharedMethods, ILoggerManager logger)
		{
			_ifrfContext = ifrfContext;
			_sharedMethods = sharedMethods;
			_logger = logger;
		}

		public List<SrPositionAggregateResponse> GenerateSrPositionAggregate(long subrackId)
		{
			List<SrPositionAggregateResponse> responseCollection = new List<SrPositionAggregateResponse>();


			Subrack sr = _ifrfContext.Subracks.SingleOrDefault(sr => sr.Id == subrackId);

			if (sr == null)
			{
				return null; // null to generate an Ok response with error in controller
			}


			IEnumerable<SrPosition> srPositions = _ifrfContext.SrPositions
				.Include(srp => srp.Resource)
				.Where(p => p.SubrackId == subrackId);

			var groups = srPositions.GroupBy(p => new { p.PosGroup, p.Type });

			foreach (var group in groups)
            {
				int nrFree = 0;
				int nrUsed = 0;
				SrPosition OltPosition = null;
				string OltPosName = null;

				foreach( SrPosition aPosition in group)
                {
					IEnumerable<long?> AllExtConnections = null;

					switch (sr.Type)
                    {
						case "SPL":
							// looking up from SPLI and down for SPLO
							IEnumerable<long?> FromExtConnections = _ifrfContext.Connections
								.Where(c => (c.FromId == aPosition.Id) && c.Type != "INT")
								.Select(c => c.ToId);

							IEnumerable<long?> ToExtConnections = _ifrfContext.Connections
								.Where(c => (c.ToId == aPosition.Id) && c.Type != "INT")
								.Select(c => c.FromId);
							AllExtConnections = FromExtConnections.Union(ToExtConnections).ToList();
							break;
						case "PP":
						case "OLT":
							// looking downstream
							AllExtConnections = _ifrfContext.Connections
								.Where(c => (c.FromId == aPosition.Id) && c.Type != "INT")
								.Select(c => c.ToId);
							break;
						default:
							// looking downstream as well
							AllExtConnections = _ifrfContext.Connections
								.Where(c => (c.FromId == aPosition.Id) && c.Type != "INT")
								.Select(c => c.ToId);
							break;

					}



					bool hasExtConnections = AllExtConnections.Any();
					bool hasStatusFree = aPosition.Resource.OperationalStatus.Equals("O");

					if (hasExtConnections) nrUsed++;
					if (!hasExtConnections && hasStatusFree) nrFree++;

					if (aPosition.Type.Equals("SPLI") && hasExtConnections && AllExtConnections.Count() == 1)
                    {
						(OltPosition,OltPosName) = _sharedMethods.TraceSplInToOlt(aPosition.Id);
						if (OltPosition != null) OltPosition.Subrack = null; // avoid returning too much info

                    }
                }
				SrPositionAggregateResponse aResponse = new SrPositionAggregateResponse()
				{
					NrTotalPositions = group.Count(),
					NrFreePositions = nrFree,
					NrUsedPositions = nrUsed,
					PosGroup = group.Key.PosGroup,
					Type = group.Key.Type,
					OltPosition = OltPosition,
					OltPositionName = OltPosName,
				};
				responseCollection.Add(aResponse);
            }

			responseCollection = responseCollection.OrderBy(c => c.PosGroup).ThenBy(c => c.Type).ToList();
			return responseCollection;
		}
	}
}
