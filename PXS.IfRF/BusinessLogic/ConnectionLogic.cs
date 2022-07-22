/**
 * @Name BusinessLogicHelper.cs
 * @Purpose 
 * @Date 07 September 2021, 07:32:35
 * @Author S.Deckers
 * @Description Handles Businesslogic where multiple resources are involved
 */

namespace PXS.IfRF.BusinessLogic
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;

	using PXS.IfRF.Data.Model;
	using PXS.IfRF.ErrorHandling;
	using PXS.IfRF.Logging;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using PXS.IfRF.Services;
	using PXS.IfRF.Enum;
	using d = System.Diagnostics.Debug;

	#endregion

	public class ConnectionLogic : BusinessLogic
	{
		private readonly ISettingsService _settingsService = null;

		public ConnectionLogic
		(
			ModelContext		ifrfContext
		,	ILoggerManager		logger
		) : base( ifrfContext, logger)
		{}

		public ConnectionLogic
		(
			ModelContext		ifrfContext
		,	ILoggerManager		logger
		,	ISettingsService	settingsService
		) : base( ifrfContext, logger)
		{
			this._settingsService = settingsService;
		}

		public override BusinessLogicResult BeforeCreate( SpecificResource item)
		{
			Connection newConnection = item as Connection;

			return( newInstance_ConnectionValidations( newConnection));
		}

		public override	BusinessLogicResult BeforeUpdate
		( 
			SpecificResource	patch
		,	SpecificResource	existing
		,	IEnumerable<string> changedFields
		)
		{
			BusinessLogicResult result = new BusinessLogicResult( );

			Connection existingConnection	= existing as Connection;
			Connection patchConnection		= patch as Connection;

			return( patchInstance_ConnectionValidations( existingConnection.Id, patchConnection));
		}

		public override BusinessLogicResult AfterCreate( SpecificResource specificResource)
		{
			assignUniqueSeqNrPerFromToPop( specificResource as Connection);
			
			Connection	newConnection	= specificResource as Connection;
			Resource	newResource		= newConnection.Resource;

			if( newResource?.LifecycleStatus == "A")
			{
				if( newConnection.ToId.HasValue)
				{
					Resource rsTo = _ifrfContext.RsResources.FirstOrDefault( x => x.Id == newConnection.ToId.Value );
					rsTo.OperationalStatus = "O";
				}

				if( newConnection.FromId.HasValue)
				{
					Resource rsFrom = _ifrfContext.RsResources.FirstOrDefault( x => x.Id == newConnection.FromId.Value );
					rsFrom.OperationalStatus = "O";
				}
			}

			return( new BusinessLogicResult( ));
		}

		/// <summary date="05-10-2021, 10:42:51" author="S.Deckers">
		/// Connection Validations where the input item is a new instance, t.i. all the required properties are available
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private BusinessLogicResult newInstance_ConnectionValidations( Connection item)
		{
			BusinessLogicResult result = new BusinessLogicResult();

			// --- Check lineId is unique (20210901 SDE)

			if (string.IsNullOrEmpty(item.LineId) == false)
			{
				if (lineIdIsUnique(item.Id, item.LineId) == false)
				{
					result.ErrorMessages.Add($"Line '{ item.LineId }' already taken");
				}
			}

			// --- if the Connection = "CL-C" LineId is mandatory (20211005 SDE)

			if( item.Type == "CL-C")
			{
				if( string.IsNullOrEmpty( item.LineId))
				{
					result.ErrorMessages.Add( $"LineId is mandatory");
				}
			}

			// --- From-to 2 checks :
			//		o from-to are different
			//		o from-to is unique 

			if (item.ToId.HasValue && item.FromId.HasValue)
			{
				if (item.FromId.Value == item.ToId.Value)
				{
					result.ErrorMessages.Add($"FromId and ToId need to have different values");
				}

				if (fromToIdIsUnique(id: item.Id, from: item.FromId.Value, to: item.ToId.Value) == false)
				{
					result.ErrorMessages.Add($"Combination FromId:{ item.FromId.Value } and ToId:{ item.ToId.Value } already taken");
				}
			}

			return( result);
		}

		private BusinessLogicResult patchInstance_ConnectionValidations( long existing, Connection item)
		{
			BusinessLogicResult result = new BusinessLogicResult();

			// --- Check lineId is unique (20210901 SDE)

			if (string.IsNullOrEmpty(item.LineId) == false)
			{
				if (lineIdIsUnique(existing, item.LineId) == false)
				{
					result.ErrorMessages.Add($"Line '{ item.LineId }' already taken");
				}
			}

			// --- From-to 2 checks :
			//		o from-to are different
			//		o from-to is unique 

			if (item.ToId.HasValue && item.FromId.HasValue)
			{
				if (fromToAreDifferent(from:item.FromId.Value, to: item.ToId.Value) == false)
				{
					result.ErrorMessages.Add($"FromId and ToId need to have different values");
				}

				if (fromToIdIsUnique(id: existing, from: item.FromId.Value, to: item.ToId.Value) == false)
				{
					result.ErrorMessages.Add($"Combination FromId:{ item.FromId.Value } and ToId:{ item.ToId.Value } already taken");
				}
			}

			return( result);
		}

		private bool fromToAreDifferent( long from, long to)
		{
			if( from == to)
			{
				return( false);
			}

			return (true);
		}

		private bool lineIdIsUnique(long id, string lineId)
		{
			if (this._ifrfContext.Connections.Where(x => x.Id != id && x.LineId == lineId).Count() > 0)
			{
				return (false);
			}

			return (true);
		}

		private bool fromToIdIsUnique(long id, long from, long to)
		{
			if (this._ifrfContext.Connections.Where(x => x.Id != id && x.FromId == from && x.ToId == to).Count() > 0)
			{
				return (false);
			}

			return (true);
		}

		/// <summary date="30-09-2021, 21:23:17" author="S.Deckers">
		/// Assigns the unique seq nr per from to pop.
		/// </summary>
		/// <param name="srPosition">The sr position.</param>
		private void assignUniqueSeqNrPerFromToPop( Connection connection)
		{
			if( string.IsNullOrEmpty( connection.FromPopName))
			{ 
				return;
			}

			if( string.IsNullOrEmpty( connection.ToPopName))
			{ 
				return;
			}

			string prefix = this._settingsService.GetSeqNrPrefix( SequenceNumbers.Connection);

			Connection item = this._ifrfContext.Connections.Where( x => x.FromPopName == connection.FromPopName 
																&& x.ToPopName == connection.ToPopName 
																&& x.Nr != null).OrderByDescending( x => x.Id).FirstOrDefault();

			if( item == null)
			{ 
				connection.Nr = $"{ prefix }1";
				return;
			}

			try
			{
				string[] parts = item.Nr.Split( prefix);

				long nextNum = System.Convert.ToInt64( parts[ parts.Length - 1]);
				nextNum++;

				connection.Nr = $"{ prefix }{ nextNum}";
			}
			catch( System.Exception)
			{ 
			};
		}

		public override BusinessLogicResult AfterUpdate( SpecificResource before, SpecificResource after, IEnumerable<string> changedFields )
		{
			Connection cbefore	= before as Connection;
			Connection cafter	= after as Connection;

			// --- Whenever a lifecycle status of a connection is set to 'A' we set the operational status
			//     of connected items to 'O'

			Resource theResource = _ifrfContext.RsResources.FirstOrDefault(x => x.Id == cafter.Id); 

			if( theResource?.LifecycleStatus == "A")
			{
				if( cafter.ToId.HasValue)
				{
					Resource rsTo = _ifrfContext.RsResources.FirstOrDefault( x => x.Id == cafter.ToId.Value );
					rsTo.OperationalStatus = "O";
				}

				if( cafter.FromId.HasValue)
				{
					Resource rsFrom = _ifrfContext.RsResources.FirstOrDefault( x => x.Id == cafter.FromId.Value );
					rsFrom.OperationalStatus = "O";
				}
			}

			return new BusinessLogicResult();
		}

		public override BusinessLogicResult AfterDelete(SpecificResource resource)
        {
			Connection deletedConnection = resource as Connection;
			Resource deletedResource = _ifrfContext.RsResources.FirstOrDefault(x => x.Id == deletedConnection.Id); 
			
			if (deletedConnection.Type == "CL-C" && new[] { "PD", "A", null, "NA", "P" }.Contains(deletedResource.LifecycleStatus))
            {
				if (deletedConnection.ToId.HasValue) //ONTP position will receive stanby status, we assume there is no other connection present.
				{
					Resource rsTo = _ifrfContext.RsResources.FirstOrDefault(x => x.Id == deletedConnection.ToId.Value);
					SrPosition position = _ifrfContext.SrPositions.FirstOrDefault(x => x.Id == rsTo.Id); 

					if (position.Type == "ONTP")
                    {
						rsTo.OperationalStatus = "STBY";
					}
				}
			}

			return new BusinessLogicResult();
		}

		public override BusinessLogicResult ValidateCreate( SpecificResource newEntity )
		{
			throw new System.NotImplementedException( );
		}

		public override BusinessLogicResult ValidateEdit(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
        {
            throw new System.NotImplementedException();
        }

        public override BusinessLogicResult ValidateDelete(SpecificResource resource)
        {
            throw new System.NotImplementedException();
        }

        internal BusinessLogicResult AfterUpdateConnectionResource(Resource before, Resource after, IEnumerable<string> changedFields)
        {
            if (string.IsNullOrEmpty(after.LifecycleStatus))
            {
                return null;
            }

            if (after.LifecycleStatus != "A")
            {
                return null;
            }

            Connection connection = _ifrfContext.Connections.FirstOrDefault(x => x.Id == before.Id);

            return AfterUpdate(connection, connection, new List<string>());
        }
    }
}