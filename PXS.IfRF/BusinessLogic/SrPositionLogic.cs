/**
 * @Name SrPositionLogic.cs
 * @Purpose 
 * @Date 30 September 2021, 19:35:37
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.BusinessLogic
{
	#region -- Using directives --
	using System;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Logging;
	using System.Collections.Generic;
	using System.Linq;
	using System.Data;
	using PXS.IfRF.Services;
	using d = System.Diagnostics.Debug;
	#endregion

	public class SrPositionLogic : BusinessLogic
	{
		private readonly ISettingsService _settingsService = null;

		public SrPositionLogic
		(
			ModelContext		ifrfContext
		,	ILoggerManager		logger
		,	ISettingsService	settingsService
		) : base(ifrfContext, logger)
		{
			this._settingsService = settingsService;
		}

		/// <summary date="02-10-2021, 09:17:24" author="S.Deckers">
		/// BeforeCreate validations
		/// </summary>
		/// <param name="before">Instance before update</param>
		/// <returns></returns>
		public override BusinessLogicResult BeforeCreate( SpecificResource item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			SrPosition newOntp = item as SrPosition;

			if( newOntp.Type != "ONTP")
			{
				return( new BusinessLogicResult( ));
			}

			return( newInstance_ONTPValidations( newOntp));
		}

		/// <summary date="02-10-2021, 09:54:51" author="S.Deckers">
		/// BeforeUpdate validations
		/// </summary>
		/// <param name="before">Instance before update</param>
		/// <returns></returns>
		public override	BusinessLogicResult BeforeUpdate
		( 
			SpecificResource	patch
		,	SpecificResource	existing
		,	IEnumerable<string> changedFields
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			SrPosition existingOntp = existing as SrPosition;
			SrPosition patchOntp	= patch as SrPosition;
			
			if( existingOntp.Type == "ONTP")
			{
				// --- Type is unchanged, only the values are changed. This is the normal case (20211002 SDE)

				if( string.IsNullOrEmpty( patchOntp.Type))
				{
					return( patchInstance_ONTPValidations( existingOntp.Id, patchOntp));
				}

				// --- Type is changed from ONTP to ONTP, normal validations apply (20211002 SDE)

				if( patchOntp.Type == "ONTP")
				{
					return( patchInstance_ONTPValidations( existingOntp.Id, patchOntp));
				}

				// --- Type is changed from ONTP to something else, we always allow that (20211002 SDE)

				return( new BusinessLogicResult( ));
			}

			// --- Existing type != 'ONTP', user doesn't change the type, we always allow that (20211002 SDE)

			if( string.IsNullOrEmpty( patchOntp.Type ) )
			{
				return( new BusinessLogicResult( ));
			}

			// ---- User changes type to 'ONTP', we need to validate (20211002 SDE)

			if( patchOntp.Type == "ONTP")
			{
				return( patchInstance_ONTPValidations( existingOntp.Id, patchOntp));
			}

			// ---- SPLI -> SPLO : existing type != 'ONTP' user changes to something else then 'ONTP', we need to validate (20211002 SDE)

			return (new BusinessLogicResult( ));
		}

		/// <summary date="02-10-2021, 09:54:11" author="S.Deckers">
		/// ONTP Validations where the input item is a new instance, t.i. all the required properties are available
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private BusinessLogicResult newInstance_ONTPValidations( SrPosition item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			BusinessLogicResult result = new BusinessLogicResult();

			if( string.IsNullOrEmpty( item.Utac) == false)
			{
				if( utacIsUnique( -1, item.Utac) == false)
				{
					result.ErrorMessages.Add( $"Utac '{ item.Utac }' already taken");
				}
			}

			// --- PositionId is Mandatory (20211002 SDE)

			if( string.IsNullOrEmpty( item.PositionId) )
			{
				result.ErrorMessages.Add( $"PositionId is Mandatory");
			}

			// --- PositionId needs to be unique (20211002 SDE)

			else
			{
				if( positionIdIsUnique( -1, item.PositionId) == false)
				{
					result.ErrorMessages.Add( $"PositionId ''{ item.PositionId }' already taken");
				}
			}

			return( result);
		}

		/// <summary date="02-10-2021, 09:34:18" author="S.Deckers">
		/// ONTP Validations where the input item is a patch instance, t.i. only values to be updated are available
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private BusinessLogicResult patchInstance_ONTPValidations( long existing, SrPosition item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			BusinessLogicResult result = new BusinessLogicResult();

			if( string.IsNullOrEmpty( item.Utac) == false)
			{
				if( utacIsUnique( existing, item.Utac) == false)
				{
					result.ErrorMessages.Add( $"Utac '{ item.Utac }' already taken");
				}
			}

			// --- PositionId is Mandatory (20211002 SDE)

			if( string.IsNullOrEmpty( item.PositionId) )
			{
				result.ErrorMessages.Add( $"PositionId is Mandatory");
			}

			// --- PositionId needs to be unique (20211002 SDE)

			else
			{
				if( positionIdIsUnique( existing, item.PositionId) == false)
				{
					result.ErrorMessages.Add( $"PositionId '{ item.PositionId }' already taken");
				}
			}

			return( result);
		}

		/// <summary date="02-10-2021, 10:54:11" author="S.Deckers">
		/// Checks if a utac not has been taken
		/// </summary>
		/// <param name="existing">The existing.</param>
		/// <param name="theUtac">The utac.</param>
		/// <returns></returns>
		private bool utacIsUnique( long existing, string theUtac)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			if( this._ifrfContext.SrPositions.Where( x => x.Type == "ONTP" && x.Id != existing && x.Utac == theUtac).Count() > 0)
			{
				return( false);
			};
			return( true);
		}

		/// <summary date="02-10-2021, 20:52:23" author="S.Deckers">
		/// Checks if a PositionId is unique
		/// </summary>
		/// <param name="existing">The existing.</param>
		/// <param name="thePositionId">The position identifier.</param>
		/// <returns></returns>
		private bool positionIdIsUnique( long existing, string thePositionId)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			if( this._ifrfContext.SrPositions.Where( x => x.Type == "ONTP" && x.Id != existing && x.PositionId == thePositionId).Count() > 0)
			{
				return( false);
			};
			return( true);
		}

		public override BusinessLogicResult AfterCreate( SpecificResource specificResource)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			BusinessLogicResult businessLogicResult = assignUniqueSeqNrPerSubrack( specificResource as SrPosition);

			return ( businessLogicResult);
		}

		/// <summary date="27-09-2021, 13:44:18" author="S.Deckers">
		/// Assigns the unique seq nr per pop.
		/// </summary>
		/// <param name="subRack">The sub rack.</param>
		private BusinessLogicResult assignUniqueSeqNrPerSubrack( SrPosition srPosition)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			BusinessLogicResult brusinessLogicResult = new BusinessLogicResult( );
			if( srPosition.SubrackId.HasValue == false)
			{
				return( brusinessLogicResult);
			}

			string prefix = this._settingsService.GetSeqNrPrefix( PXS.IfRF.Enum.SequenceNumbers.Position);

			SrPosition item = this._ifrfContext.SrPositions.Where( x => x.SubrackId == srPosition.SubrackId && x.SeqNr != null).OrderByDescending( x => x.Id).FirstOrDefault();

			if( item == null)
			{ 
				srPosition.SeqNr = $"{ prefix }1";
				return( brusinessLogicResult);
			}

			try
			{
				string[] parts = item.SeqNr.Split( prefix);

				long nextNum = System.Convert.ToInt64( parts[ parts.Length - 1]);
				nextNum++;
				srPosition.SeqNr = $"{ prefix }{ nextNum}";
				return( brusinessLogicResult);
			}
			catch( System.Exception ex)
			{
				brusinessLogicResult.ErrorMessages.Add( $"Error determining new sequence nr:{ ex.Message }");
				return( brusinessLogicResult);
			};
		}

		public override BusinessLogicResult AfterDelete(SpecificResource resource)
		{
			return null;
		}

		public override BusinessLogicResult AfterUpdate(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return null;
		}

		public override BusinessLogicResult ValidateCreate( SpecificResource newEntity )
		{
			return null;
		}

		public override BusinessLogicResult ValidateDelete(SpecificResource resource)
		{
			return( new BusinessLogicResult());
		}

		public override BusinessLogicResult ValidateEdit(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return null;
		}
	}
}