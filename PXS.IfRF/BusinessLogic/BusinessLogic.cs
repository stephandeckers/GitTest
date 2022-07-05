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
	using PXS.IfRF.Logging;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Http;
	#endregion

    public abstract class BusinessLogic
    {
        protected readonly ModelContext     _ifrfContext;
        protected readonly HttpContext		_httpContext;
        protected readonly ILoggerManager   _logger;

        protected BusinessLogic
        (
			ModelContext	ifrfContext
		,	ILoggerManager	logger
		)
        {
            _ifrfContext    = ifrfContext;
			_logger         = logger;
        }

        protected BusinessLogic
		(
			ModelContext			ifrfContext
		,	IHttpContextAccessor	httpContextAccessor
		,	ILoggerManager			logger
		)
        {
            _ifrfContext	= ifrfContext;
			_httpContext	= httpContextAccessor.HttpContext;
            _logger			= logger;
        }

        public abstract BusinessLogicResult ValidateCreate	( SpecificResource newEntity);
        public abstract BusinessLogicResult ValidateEdit	( SpecificResource before, SpecificResource after, IEnumerable<string> changedFields);
        public abstract BusinessLogicResult ValidateDelete	( SpecificResource resource);
		public virtual	BusinessLogicResult BeforeCreate	( SpecificResource specificResource) 
			{ return( new BusinessLogicResult( )); }
        public abstract BusinessLogicResult AfterCreate		( SpecificResource specificResource);

		public virtual	BusinessLogicResult BeforeUpdate	( SpecificResource before, SpecificResource existing, IEnumerable<string> changedFields) 
			{ return( new BusinessLogicResult( )); }
        public abstract BusinessLogicResult AfterUpdate		( SpecificResource before, SpecificResource after, IEnumerable<string> changedFields);
        public abstract BusinessLogicResult AfterDelete		( SpecificResource resource);
    }
}