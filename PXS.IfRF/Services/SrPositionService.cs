/**
 * @Name SrPositionService.cs
 * @Purpose 
 * @Date 27 July 2021, 19:34:24
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	using PXS.IfRF.Logging;
	using PXS.IfRF.AuthHandling;
	using d = System.Diagnostics.Debug;
	using Microsoft.AspNetCore.Http;
	using System.Collections.Generic;
	using System;
	using System.Data.Common;
	using Microsoft.EntityFrameworkCore;
	using System.Data;
	using Oracle.ManagedDataAccess.Client;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Supporting;
	#endregion

	public class SrPositionService : ISrPositionService
	{
		private readonly ModelContext		_ifrfContext		= null;
		private readonly ILoggerManager		_logger				= null;
		private readonly SrPositionLogic	_srPositionLogic	= null;
		private readonly IResourceService	_resourceService;
		private readonly ISharedMethods		_sharedMethods;

		public SrPositionService
		(
			ModelContext		ifrfContext
		,	ISharedMethods		sharedMethods
		,	IResourceService	resourceService
		,	ILoggerManager		logger
		,	ISettingsService	settingsService
		)
		{
			_ifrfContext		= ifrfContext;
			_sharedMethods		= sharedMethods;
			_logger				= logger;
			_resourceService	= resourceService;

			_srPositionLogic	= new SrPositionLogic(_ifrfContext, _logger, settingsService);
		}

		public SingleResult<SrPosition> Get(long id)
		{
            return SingleResult.Create(_ifrfContext.SrPositions.Where(a => a.Id == id));
		}

		/// <summary date="27-07-2021, 19:41:12" author="S.Deckers">
		/// Read items from db
		/// </summary>
		public IQueryable<SrPosition> GetItems()
		{
			return (_ifrfContext.SrPositions);
		}

		public BusinessLogicResult Delete( long id )
		{
			SrPosition	item		= _ifrfContext.SrPositions.SingleOrDefault(x => x.Id == id);
			Resource	resource	= _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);

			if (item == null && resource == null)
			{
				return new BusinessLogicResult();
			}

			BusinessLogicResult validationResult = this._srPositionLogic.ValidateDelete( item);
			if (validationResult.Succeeded)
			{
				_ifrfContext.Remove			( item);
				_ifrfContext.Remove			( resource);
				_srPositionLogic.AfterDelete( item);
				_ifrfContext.SaveChanges	( );
			}
			return validationResult;
		}

		public (BusinessLogicResult, SrPosition) Patch( long id, Delta<SrPosition> deltaItem)
		{
			SrPosition existing = _ifrfContext.SrPositions.SingleOrDefault( x => x.Id == id);

			if( existing == null)
			{
				BusinessLogicResult blr = new BusinessLogicResult();
				blr.ErrorMessages.Add( $"item { id } not found");
				return( blr, null);
			}

			BusinessLogicResult businessLogicResult = _srPositionLogic.BeforeUpdate( deltaItem.GetInstance(), existing, deltaItem.GetChangedPropertyNames());
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, existing);
			}

			deltaItem.Patch( existing);

			Resource	resource	= _ifrfContext.RsResources.SingleOrDefault( x => x.Id == id);
			_resourceService.UpdateResourceMetadataFields( resource);

			_ifrfContext.SaveChanges();
			return( businessLogicResult, existing);
		}

		public (BusinessLogicResult, SrPosition) Create ( SrPosition item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			BusinessLogicResult businessLogicResult = _srPositionLogic.BeforeCreate( item);
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, item);
			}

			using (var dbContextTransaction = this._ifrfContext.Database.BeginTransaction())
			{
				try
				{
					Resource resource = _resourceService.CreateNewResource(item);
					//making it possible to add resource info in the request
					if (item.Resource != null)
					{
						resource.OperationalStatus = item.Resource.OperationalStatus;
						resource.LifecycleStatus = item.Resource.LifecycleStatus;
						resource.OrderId = item.Resource.OrderId;
					}
					item.Resource = resource;
					businessLogicResult = _srPositionLogic.AfterCreate( item);

					if( businessLogicResult.Succeeded == false)
					{
						return ( businessLogicResult, item);
					}
					_ifrfContext.Add(item);
					_ifrfContext.SaveChanges();

					dbContextTransaction.Commit();
					return ( businessLogicResult, item);
				}
				catch (System.Exception ex)
				{
					this._logger.Error(ex.Message);
					dbContextTransaction.Rollback();
					throw;
				}
				finally
				{
				}
			}
		}

        public List<PositionConnectionSearchResponse> GetPositionsConnected(long id)
        {
            List<PositionConnectionSearchResponse> responseCollection = getPositionsConnected(id);
            return responseCollection;
        }

        private List<PositionConnectionSearchResponse> getPositionsConnected(long id)
        {
            List<PositionConnectionSearchResponse> responseCollection = new List<PositionConnectionSearchResponse>();

            try
            {
                //fetch subrack
                Subrack subrack = new Subrack();
                subrack = _ifrfContext.Subracks.Where(x => x.Id == id).FirstOrDefault();
                subrack.SrPositions = _ifrfContext.SrPositions.Where(x => x.SubrackId == id).ToHashSet();
                subrack.Pop = _ifrfContext.Pops.Where(x => x.Id == subrack.PopId).FirstOrDefault();
                subrack.Pop.RfArea = _ifrfContext.RfAreas.Where(x => x.Id == subrack.Pop.RfAreaId).FirstOrDefault();

				string owner = subrack.Pop.RfArea?.Owner;

                foreach (SrPosition srPosition in subrack.SrPositions)
                {
                    string positionType = "";
                    string positionType2 = "";
                    SrPosition relatedPosition1 = new SrPosition();
                    SrPosition relatedPosition2 = new SrPosition();

                    if (srPosition.Type == "PP")
                    {
                        positionType = "SPLI";
                        relatedPosition1 = getRelatedPositionInfo(srPosition, positionType, positionType2, owner);
                    }
                    else if (srPosition.Type == "GPON" || srPosition.Type == "XGSP" || srPosition.Type == "NT")
                    {
                        //fetch both PP position (position 1) followed by SPLI position (position2)
                        positionType = "PP";
                        relatedPosition1 = getRelatedPositionInfo(srPosition, positionType, positionType2, owner);
                        positionType = "SPLI";
                        relatedPosition2 = getRelatedPositionInfo(srPosition, positionType, positionType2, owner);
                    }
                    else if (srPosition.Type == "SPLI")
                    {
                        //fetch both PP position (position 1) followed by OLT position (position2)
                        positionType = "PP";
                        relatedPosition1 = getRelatedPositionInfo(srPosition, positionType, positionType2, owner);

                        positionType = "GPON";
                        positionType2 = "XGSP";
                        relatedPosition2 = getRelatedPositionInfo(srPosition, positionType, positionType2, owner);
                    }
                    else if (srPosition.Type == "SPLO")
                    {
                        positionType = "ONTP";
                        relatedPosition1 = getRelatedPositionInfo(srPosition, positionType, positionType2, owner);
                    }

                    srPosition.Subrack.Name = _sharedMethods.GetSubrackName(subrack: srPosition.Subrack, owner: owner);
                    srPosition.PositionName = _sharedMethods.GetPositionName(position: srPosition, owner: owner);
                    srPosition.Resource = _ifrfContext.RsResources.Where(x => x.Id == srPosition.Id).FirstOrDefault();
                    srPosition.ConnectionFroms = _ifrfContext.Connections.Where(x => x.FromId == srPosition.Id).ToHashSet(); 
                    srPosition.ConnectionTos = _ifrfContext.Connections.Where(x => x.ToId == srPosition.Id).ToHashSet();

                    // have to build new position to avoid redundant response???
                    SrPosition subrackPosition = new SrPosition();
                    //position + resource
                    subrackPosition.Id = srPosition.Id;
                    subrackPosition.Type = srPosition.Type;
                    subrackPosition.PositionId = srPosition.PositionId;
                    subrackPosition.PositionName = srPosition.PositionName;
                    subrackPosition.PosGroup = srPosition.PosGroup;
                    subrackPosition.Resource = srPosition.Resource;
                    subrackPosition.LineId = srPosition.LineId;
                    subrackPosition.Comments = srPosition.Comments;
                    subrackPosition.PxsId = srPosition.PxsId;
                    subrackPosition.ConnectorType = srPosition.ConnectorType;

                    //subrack
                    subrackPosition.Subrack = new Subrack();
                    subrackPosition.Subrack.Id = srPosition.Subrack.Id;
                    subrackPosition.Subrack.Name = srPosition.Subrack.Name;
                    subrackPosition.Subrack.SubrackId = srPosition.Subrack.SubrackId;

                    //pop
                    subrackPosition.Subrack.Pop = new Pop();
                    subrackPosition.Subrack.Pop.Id = srPosition.Subrack.Pop.Id;
                    subrackPosition.Subrack.Pop.Name = srPosition.Subrack.Pop.Name;
                    subrackPosition.Subrack.Pop.Type = srPosition.Subrack.Pop.Type;

                    Connection connection = getConnection(srPosition);

					if( relatedPosition1.Id == 0) relatedPosition1 = null;
					if( relatedPosition2.Id == 0) relatedPosition2 = null;

					PositionConnectionSearchResponse responseItem = new PositionConnectionSearchResponse()
					{
						SubrackPosition		= subrackPosition
					,	RelatedPosition1	= relatedPosition1
					,	RelatedPosition2	= relatedPosition2
					,	ConnectionInfo		= connection
					};

                    responseCollection.Add(responseItem);
                }

                // --- Construct response (20210731 SDE)

                return (responseCollection);
            }
            catch (System.Exception ex)
            {
                d.WriteLine(string.Format("Exception:{0}", ex.ToString()));
                return (responseCollection);

            }
        }

        private Connection getConnection(SrPosition srPosition)
        {
            Connection connectionInfo = new Connection(); 

            if (srPosition.Type == "SPLI" && srPosition.ConnectionTos.Count != 0)
            {
                Connection connection = _ifrfContext.Connections.Where(x => x.Id == srPosition.ConnectionTos.FirstOrDefault().Id).FirstOrDefault();
                connectionInfo.Id = connection.Id;
                connectionInfo.LineId = connection.LineId;
                connectionInfo.Nr = connection.Nr;
                connectionInfo.Type = connection.Type;
            }
            else
            {
                if (srPosition.ConnectionFroms.Count != 0)
                {
                    Connection connection = _ifrfContext.Connections.Where(x => x.Id == srPosition.ConnectionFroms.FirstOrDefault().Id).FirstOrDefault();
                    connectionInfo.Id = connection.Id;
                    connectionInfo.LineId = connection.LineId;
                    connectionInfo.Nr = connection.Nr;
                    connectionInfo.Type = connection.Type;
                }
            }
            return connectionInfo;
        }

        private SrPosition getRelatedPositionInfo(SrPosition subrackPosition, string positionType, string positionType2, string owner)
        {
            SrPosition relatedItem = new SrPosition();
            string sql = "";
            if (positionType == "ONTP") //trace does not work downstream so for SPLO we rely on connection entity, we assume direction (SPLO FROM , ONTP TO)
            {
                sql =
           @"
				select to_id as id
					from connection 
					inner join sr_position on sr_position.id = connection.to_id
					where connection.from_id = :resource_id
					and (sr_position.type = :position_type OR sr_position.type = :position_type2)
			";
            }
            else if (subrackPosition.Type == "SPLI") // trace upstream 
            { sql =
           @"
				select distinct(sr_position.id)
					from TABLE( NetworkPathOrder.getConnectedResource( res_id => :resource_id)) cp
					inner join sr_position on sr_position.id = cp.from_id
				    where 1=1
					and (sr_position.type = :position_type OR sr_position.type = :position_type2)
					and 2=2
			";
            }
            else 
            {
                sql =
           @"
				select distinct(sr_position.id)
					from TABLE( NetworkPathOrder.getConnectedResource( res_id => :resource_id)) cp
					inner join sr_position on sr_position.id = cp.to_id
				    where 1=1
					and (sr_position.type = :position_type OR sr_position.type = :position_type2)
					and 2=2
			";
            }
            DbConnection connection = _ifrfContext.GetOpenConnection();

            try
            {

                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add(new OracleParameter() { ParameterName = "resource_id", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = subrackPosition.Id });
                cmd.Parameters.Add(new OracleParameter() { ParameterName = "position_type", DbType = DbType.String, Direction = ParameterDirection.Input, Value = positionType });
                cmd.Parameters.Add(new OracleParameter() { ParameterName = "position_type2", DbType = DbType.String, Direction = ParameterDirection.Input, Value = positionType2 });

                DbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    long id = Convert.ToInt64(reader["id"]);

                    SrPosition relatedposition = new SrPosition();
                    relatedposition = _ifrfContext.SrPositions.Where(x => x.Id == id).FirstOrDefault();

                    if (relatedposition.SubrackId != null)
                    {
                        Subrack subrack = _ifrfContext.Subracks.Where(x => x.Id == relatedposition.SubrackId).FirstOrDefault();
                        relatedposition.Subrack = subrack;
                        relatedposition.Subrack.Pop = _ifrfContext.Pops.Where(x => x.Id == relatedposition.Subrack.PopId).FirstOrDefault();
                        if (relatedposition.Subrack.RackSpaceId != null)
                        {
                            relatedposition.Subrack.RackSpace = _ifrfContext.RackSpaces.Where(x => x.Id == relatedposition.Subrack.RackSpaceId).FirstOrDefault();
                        }
                        relatedposition.Subrack.Name = _sharedMethods.GetSubrackName(subrack: relatedposition.Subrack, owner: owner);
                    }

                    relatedposition.PositionName = _sharedMethods.GetPositionName(position: relatedposition, owner: owner);


                    relatedItem.Id = relatedposition.Id;
                    relatedItem.PositionName = relatedposition.PositionName;
                    if (relatedposition.SubrackId != null)
                    {
                        relatedItem.Subrack = new Subrack();
                        relatedItem.Subrack.Id = relatedposition.Subrack.Id;
                        relatedItem.Subrack.Pop = new Pop();
                        relatedItem.Subrack.Pop.Id = relatedposition.Subrack.Pop.Id;
                        relatedItem.Subrack.Pop.Name = relatedposition.Subrack.Pop.Name;
                    }


                }

                return (relatedItem);


            }
            catch (System.Exception ex)
            {
                d.WriteLine(string.Format("Exception:{0}", ex.ToString()));
                return (relatedItem);
            }

        }
    }
}
