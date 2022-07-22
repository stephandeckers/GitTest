namespace PXS.IfRF.Controllers
{
    #region -- Using directives --
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Microsoft.AspNet.OData;
    using PXS.IfRF.Services;
    using System.Collections.Generic;
    using PXS.IfRF.Logging;
    using PXS.IfRF.Data.Model;
    using PXS.IfRF.ErrorHandling;
    using System.Linq;
    #endregion

    public partial class RefdataController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Reference data and picklists";
        public RefdataController(
            IRefdataService refdataService, 
            ILoggerManager logger)
            : base (logger, refdataService)
        {
        }

        [HttpGet("{refDataType}")]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve entries of Refdata type by providing its name")]
        public IQueryable<RefPl> GetRefdatas(string refDataType)
        {
            var localItems = _refdataService.GetRefdatas(refDataType);
            return localItems;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("{refDataType}/{key}")]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve entry from Refdata type by providing its key and type name")]
        public ActionResult<RefPl> GetRefdata(string refDataType, string key)
        {
            RefPl item = _refdataService.GetRefdata(refDataType, key);

            if (item == null)
            {
				return Ok( new OperationFailureResult( $"Type '{ refDataType }' with key '{key}' not found") );
            }

            return Ok(item);
        }

		/// <summary date="21-01-2022, 11:07:15" author="S.Deckers">
		/// Gets the free splitter spare positions.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		[ HttpGet			( "RefPlOwners/sparepositions/{key}")]
        [ EnableQuery		( MaxExpansionDepth = 4)]
        [ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve SparePositions to be used in 'FreeSplitterPositionSearch'")]
        public ActionResult<string> GetFreeSplitterSparePositions(string key)
        {
			const string refDataType = "RefPlOwners";

            RefPlOwner item = _refdataService.GetRefdata( refDataType, key) as RefPlOwner;

            if( item == null)
            {
				return Ok( new OperationFailureResult( $"Type '{ refDataType }' with key '{key}' not found") );
            }			

			if( string.IsNullOrEmpty( item.SparePositions))
			{
				return Ok( item.SparePositions);
			}

			if( item.SparePositions.Contains( ",") == false)
			{
				return Ok( item.SparePositions);
			}

			string[] positions = item.SparePositions.Split( ",");

			return Ok( positions.ToList());
        }
		/*
		[ HttpGet			( "RefPlOwners/sparepositions/{key}")]
        [ EnableQuery		( MaxExpansionDepth = 4)]
        [ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve SparePositions to be used in 'FreeSplitterPositionSearch'")]
        public ActionResult<List<int>> GetFreeSplitterSparePositions( string key)
        {
			const string refDataType = "RefPlOwners";

            RefPlOwner	item			= _refdataService.GetRefdata( refDataType, key) as RefPlOwner;
			List<int>	thePositions	= new List<int>( );

            if( item == null)
            {
				return Ok( new OperationFailureResult( $"Type '{ refDataType }' with key '{key}' not found") );
            }		

			if( item.SparePositions.Contains( ",") == false)
			{
				if( System.Int32.TryParse( item.SparePositions, out int theInteger))
				{
					thePositions.Add( theInteger);
				}

				return Ok( thePositions);
			}

			string[] positions = item.SparePositions.Split( ",");
			
			foreach( string pos in positions)
			{
				if( System.Int32.TryParse( pos, out int theInteger))
				{
					thePositions.Add( theInteger);
				}
			}

			return Ok( thePositions);
        }*/
    }
}

