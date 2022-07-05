/**
 * @Name PopLogic.cs
 * @Purpose 
 * @Date 06 October 2021, 06:48:33
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
	using System.Data;
	using Microsoft.AspNetCore.Http;
	#endregion

	public class PopLogic : BusinessLogic
	{
		public PopLogic
		(
			ModelContext			ifrfContext
		,	IHttpContextAccessor	httpContextAccessor
		,	ILoggerManager			logger
		) : base( ifrfContext, httpContextAccessor, logger)
		{}

		/// <summary date="02-10-2021, 09:17:24" author="S.Deckers">
		/// BeforeCreate validations
		/// </summary>
		/// <param name="before">Instance before update</param>
		/// <returns></returns>
		public override BusinessLogicResult BeforeCreate( SpecificResource item)
		{
			Pop newPop = item as Pop;

			return( newInstance_POPValidations( newPop));
		}

		/// <summary>
		/// Befores the update.
		/// </summary>
		/// <param name="patch">The patch.</param>
		/// <param name="existing">The existing.</param>
		/// <param name="changedFields">The changed fields.</param>
		/// <returns></returns>
		public override	BusinessLogicResult BeforeUpdate
		( 
			SpecificResource	patch
		,	SpecificResource	existing
		,	IEnumerable<string> changedFields
		)
		{
			Pop existingItem = existing as Pop;
			Pop patchingItem = patch as Pop;
			
			return( patchInstance_POPValidations( existingItem, patchingItem));
		}

		private BusinessLogicResult newInstance_POPValidations( Pop item)
		{
			BusinessLogicResult result = new BusinessLogicResult();

			if( ! string.IsNullOrEmpty( item.Name) )
			{
				if( nameIsUnique( -1, item.Name) == false)
				{
					result.ErrorMessages.Add( $"Name '{ item.Name }' already taken");
				}
			}

			// --- Set timestamps for popstatus on creation (20220509 SDE)

			item.PopStatusModified		= System.DateTime.Now;
			item.PopStatusModifiedBy	= (string)(_httpContext.Items["iv-user"]);

			return( result);
		}

		/// <summary date="02-10-2021, 09:34:18" author="S.Deckers">
		/// ONTP Validations where the input item is a patch instance, t.i. only values to be updated are available
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private BusinessLogicResult patchInstance_POPValidations( Pop existing, Pop item)
		{
			BusinessLogicResult result = new BusinessLogicResult();

			// --- Pop-name and RfAreaId are changed (20211007 SDE)

			if( ! string.IsNullOrEmpty( item.Name))
			{
				if( nameIsUnique( existing.Id, item.Name) == false)
				{
					result.ErrorMessages.Add( $"Name '{ item.Name }' already taken");
				}
			}

			// --- Set modified-info if PopStatus is modified (20220502 SDE)

			if( ! string.IsNullOrEmpty( item.Status))
			{				
				existing.PopStatusModified		= System.DateTime.Now;
				existing.PopStatusModifiedBy	= (string)(_httpContext.Items["iv-user"]);
			}

			return( result);
		}

		private bool nameIsUnique( long existing, string theName)
		{
			if( this._ifrfContext.Pops.Where( x => x.Name == theName && x.Id != existing).Count() > 0)
			{
				return( false);
			};
			return( true);
		}

		#region -- Not implemented --
		public override BusinessLogicResult AfterCreate( SpecificResource specificResource)
		{
			return( null);
		}

		public override BusinessLogicResult AfterDelete( SpecificResource resource)
		{
			return( null);
		}

		public override BusinessLogicResult AfterUpdate( SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{
			return( null);
		}

		public override BusinessLogicResult ValidateCreate( SpecificResource newEntity )
		{
			return( null);
		}

		public override BusinessLogicResult ValidateDelete( SpecificResource resource)
		{
			return( null);
		}

		public override BusinessLogicResult ValidateEdit( SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
		{	
			return( null); 
		}
		#endregion
	}
}