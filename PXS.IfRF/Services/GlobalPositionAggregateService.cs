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

    public class GlobalPositionAggregateService : IGlobalPositionAggregateService
	{
		private readonly ModelContext _ifrfContext = null;
		private readonly ISharedMethods _sharedMethods;
		private readonly ILoggerManager _logger = null;

		public GlobalPositionAggregateService(ModelContext ifrfContext, ISharedMethods sharedMethods, ILoggerManager logger)
		{
			_ifrfContext = ifrfContext;
			_sharedMethods = sharedMethods;
			_logger = logger;
		}

		public (List<GlobalPositionAggregateResponse>, string) GenerateGlobalPositionAggregate(
			GlobalPositionAggregateRequest request)
		{
			string errorMessage = string.Empty;
			List<GlobalPositionAggregateResponse> responseCollection = new List<GlobalPositionAggregateResponse>();

			long rfAreaId = request.RfAreaId;
			List<string> subrackTypes = request.SubrackTypes;

			List<string> supportedSubrackTypes = new List<string>() { "OLT", "SPL" };

			if (subrackTypes == null)
            {
				subrackTypes = supportedSubrackTypes;
            }

			List<string> oltPositionTypes = new List<string>() { "GPON", "XGSP" };

			List<string> blockedStatuses = new List<string>() { "R", "DF", "B" };

			foreach (string subrackType in subrackTypes)
			{
				if (!supportedSubrackTypes.Contains(subrackType))
				{
					errorMessage = $"Subrack type {subrackType} not supported.";
					return (responseCollection, errorMessage);
				}
			}

			RfArea rfArea = _ifrfContext.RfAreas.SingleOrDefault(ar => ar.Id == rfAreaId);
			if (rfArea == null)
			{
				errorMessage = $"RfArea with id {rfAreaId} not found.";
				return (responseCollection, errorMessage);
			}

			List<GlobalPositionInfoItem> positionData = _ifrfContext.SrPositions
				.Where(p => subrackTypes.Contains(p.Subrack.Type) && p.Subrack.Pop.RfAreaId == rfAreaId)
				.Include(p => p.Resource)
				.Include(p => p.Subrack)
				.ThenInclude(sr => sr.Pop)
				.ThenInclude(pop => pop.RfArea)
				.Include(p => p.ConnectionFroms)
				.Include(p => p.ConnectionTos)
				.Select(p => new GlobalPositionInfoItem {
					Owner = p.Subrack.Pop.RfArea.Owner,
					RfAreaId = p.Subrack.Pop.RfArea.Id,
					RfAreaName = p.Subrack.Pop.RfArea.Name,
					RfAreaCode = p.Subrack.Pop.RfArea.Code,
					PopId = p.Subrack.Pop.Id,
					PopName = p.Subrack.Pop.Name,
					PopType = p.Subrack.Pop.Type,
					SubrackType = p.Subrack.Type,
					PositionId = p.Id,
					PositionType = p.Type,
					OltPositionType = oltPositionTypes.Contains(p.Type) ? p.Type : null,
					OperationalStatus = p.Resource.OperationalStatus,
					NrFroms = p.ConnectionFroms.Count(c => c.Type != "INT"),
					NrTos = p.ConnectionTos.Count(c => c.Type != "INT")
				}).ToList();

			var splitterIns = positionData.Where(p => p.PositionType == "SPLI");

			IDictionary<long,string> oltPositionTypesConnectedToSplins = _sharedMethods.TraceRfAreaSplInToOlt(rfAreaId);

			foreach (var aSplitterIn in splitterIns)
            {
				List<long?> connectedSplOutPositions = _ifrfContext.SrPositions.Include(p => p.ConnectionFroms)
					.FirstOrDefault(p => p.Id == aSplitterIn.PositionId)
					.ConnectionFroms.Select(p => p.ToId).ToList();


				string oltPositionType = string.Empty;
				if (!oltPositionTypesConnectedToSplins.TryGetValue(aSplitterIn.PositionId, out oltPositionType)) continue;

				foreach (GlobalPositionInfoItem anItem in positionData.Where(p => connectedSplOutPositions.Contains(p.PositionId)))
				{
						anItem.OltPositionType = oltPositionType;
				}
            }

			var positionDataGrouped = positionData
				.AsEnumerable()
				.GroupBy(p => new { p.Owner, p.RfAreaId, p.RfAreaName, p.RfAreaCode, p.PopType, p.PopId, p.PopName, p.SubrackType, p.PositionType, p.OltPositionType })
				.OrderBy(p => p.Key.Owner)
				.ThenBy(x => x.Key.RfAreaName)
				.ThenBy(x => x.Key.PopType)
				.ThenBy(x => x.Key.PopName)
				.ThenBy(x => x.Key.SubrackType)
				.ThenBy(x => x.Key.PositionType)
				.ThenBy(x => x.Key.OltPositionType);

			foreach (var aGroup in positionDataGrouped)
            {
				int nrTotalPositions = aGroup.Count();
				int nrUsedPositions = 0;
				int nrBlockedPositions = aGroup.Count(c => blockedStatuses.Contains(c.OperationalStatus));
				int nrFreePositions = 0; // free means not connected and not blocked
				switch (aGroup.Key.PositionType)
                {
					case "SPLI":
					case "NT":
						continue;
						//nrUsedPositions = aGroup.Count(c => c.NrTos > 0);
						//nrFreePositions = aGroup.Count(c => c.NrTos == 0 && !blockedStatuses.Contains(c.OperationalStatus));
						//break;
					case "SPLO":
					case "GPON":
					case "XGSP":
					case "PP":
					case "UNK":
						nrUsedPositions = aGroup.Count(c => c.NrFroms > 0);
						nrFreePositions = aGroup.Count(c => c.NrFroms == 0 && !blockedStatuses.Contains(c.OperationalStatus));
						break;

                }

				GlobalPositionAggregateResponse aResponse = new GlobalPositionAggregateResponse()
				{
					Owner = aGroup.Key.Owner,
					RfAreaId = aGroup.Key.RfAreaId,
					RfAreaName = aGroup.Key.RfAreaName,
					RfAreaCode = aGroup.Key.RfAreaCode,
					PopId = aGroup.Key.PopId,
					PopName = aGroup.Key.PopName,
					PopType = aGroup.Key.PopType,
					SubrackType = aGroup.Key.SubrackType,
					PositionType= aGroup.Key.PositionType,
					OltPositionType = aGroup.Key.OltPositionType,
					NrTotalPositions = nrTotalPositions,
					NrUsedPositions = nrUsedPositions,
					NrFreePositions = nrFreePositions,
					NrBlockedPositions = nrBlockedPositions
				};

				responseCollection.Add(aResponse);
            }

			return (responseCollection, errorMessage);
		}
	}
}
