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
    using PXS.IfRF.Supporting;
    #endregion

    public class FreeConsecutivePositionSearchService : IFreeConsecutivePositionSearchService
	{
		private readonly ModelContext _ifrfContext = null;
		private readonly ILoggerManager _logger = null;
		private readonly ISharedMethods _sharedMethods;

		public FreeConsecutivePositionSearchService(ModelContext ifrfContext, ISharedMethods sharedMethods, ILoggerManager logger)
		{
			_ifrfContext = ifrfContext;
			_sharedMethods = sharedMethods;
			_logger = logger;
		}

		public List<FreeConsecutivePositionSearchResponse> GetFreePositions(FreeConsectivePositionSearchRequest request)
		{
			
			List<FreeConsecutivePositionSearchResponse> responseCollection = getFreePositions(request.SubrackId, request.PositionType, request.From_To, request.Owner);
			return responseCollection;
			
		}
       

        private List<FreeConsecutivePositionSearchResponse> getFreePositions(long subrackId, string positionType, string from_to, string owner)
        {
			List<FreeConsecutivePositionSearchResponse> responseCollection = new List<FreeConsecutivePositionSearchResponse>();

			if (from_to == "FROM" || from_to == "TO")
			{
				string sql = "";
				sql = @"select sr_position.id from sr_position inner join rs_resource on sr_position.id = rs_resource.id
							where
							sr_position.subrack_id = :subrackId
							and rs_resource.operational_status = 'O'
							and sr_position.type = :positionType
							order by sr_position.pos_group, sr_position.position_id";

				DbConnection connection = _ifrfContext.GetOpenConnection();
				try
				{
					DbCommand cmd = connection.CreateCommand();
					cmd.CommandText = sql;
					cmd.Parameters.Add(new OracleParameter() { ParameterName = "subrackId", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = subrackId });
					cmd.Parameters.Add(new OracleParameter() { ParameterName = "positionType", DbType = DbType.String, Direction = ParameterDirection.Input, Value = positionType });

					DbDataReader reader = cmd.ExecuteAndLogReader(_logger);

					List<SrPosition> positionList = new List<SrPosition>();
					while (reader.Read())
					{
						long id = System.Convert.ToInt64(reader["id"]);
						SrPosition positionItem = new SrPosition();
						positionItem = this._ifrfContext.SrPositions.Where(x => x.Id == id).FirstOrDefault();
						positionItem.ConnectionFroms = this._ifrfContext.Connections.Where(x => x.FromId == id).ToHashSet();
						positionItem.ConnectionTos = this._ifrfContext.Connections.Where(x => x.ToId == id).ToHashSet();

						positionList.Add(positionItem);
					}

					if (from_to == "FROM")
                    {
                        for (int i = 0; i < positionList.Count; i++)
                        {
							int counter = 0; 
							if (positionList[i].ConnectionFroms.Count == 0)
                            {
								//calculate PositionName
								positionList[i].Subrack			= _ifrfContext.Subracks.Where(x => x.Id == positionList[i].SubrackId).FirstOrDefault();
								positionList[i].Subrack.Pop		= _ifrfContext.Pops.Where(x => x.Id == positionList[i].Subrack.PopId).FirstOrDefault();
								if (positionList[i].Subrack.RackSpaceId != null)
								{
									positionList[i].Subrack.RackSpace = _ifrfContext.RackSpaces.Where(x => x.Id == positionList[i].Subrack.RackSpaceId).FirstOrDefault();
								}
								//start calculations after full position and related entities are collected
								positionList[i].Subrack.Name = _sharedMethods.GetSubrackName(subrack: positionList[i].Subrack, owner: owner);
								positionList[i].PositionName = _sharedMethods.GetPositionName(position: positionList[i], owner: owner);
								int j = i;
								while (j < positionList.Count && positionList[j].ConnectionFroms.Count == 0 )
								{
									counter++;
									j++;
								
								}
								FreeConsecutivePositionSearchResponse responseItem = new FreeConsecutivePositionSearchResponse()
								{
									PositionId = positionList[i].Id
								,	PositionName = positionList[i].PositionName
								,	ConsecutiveFree = counter

								};
								responseCollection.Add(responseItem);
							}
                        }

						
					}
					if (from_to == "TO")
					{

						for (int i = 0; i < positionList.Count; i++)
						{
							int counter = 0;
							if (positionList[i].ConnectionTos.Count == 0)
							{
								//calculate PositionName
								positionList[i].Subrack = _ifrfContext.Subracks.Where(x => x.Id == positionList[i].SubrackId).FirstOrDefault();
								positionList[i].Subrack.Pop = _ifrfContext.Pops.Where(x => x.Id == positionList[i].Subrack.PopId).FirstOrDefault();
								if (positionList[i].Subrack.RackSpaceId != null)
								{
									positionList[i].Subrack.RackSpace = _ifrfContext.RackSpaces.Where(x => x.Id == positionList[i].Subrack.RackSpaceId).FirstOrDefault();
								}
								//start calculations after full position and related entities are collected
								positionList[i].Subrack.Name = _sharedMethods.GetSubrackName(subrack: positionList[i].Subrack, owner: owner);
								positionList[i].PositionName = _sharedMethods.GetPositionName(position: positionList[i], owner: owner);

								int j = i;
								while ( j < positionList.Count && positionList[j].ConnectionTos.Count == 0 )
								{
									
									counter++;
									j++;

								}
								FreeConsecutivePositionSearchResponse responseItem = new FreeConsecutivePositionSearchResponse()
								{
									PositionId = positionList[i].Id
											,
									PositionName = positionList[i].PositionName
											,
									ConsecutiveFree = counter

								};
								responseCollection.Add(responseItem);
							}
						}
					}

				}
				catch (System.Exception ex)
				{
					d.WriteLine(string.Format("Exception:{0}", ex.ToString()));
					return (responseCollection);
				}
				finally
				{
					if (connection.State == ConnectionState.Open)
					{
						connection.Close();
					}
				}


				return (responseCollection);



			}

			return (responseCollection);
		}

				
	}
    
}
