/**
 * @Name BusinessLogicHelper.cs
 * @Purpose 
 * This class dispatches any calls it gets from generic resources to the resource-specific classes
 * @Date 07 September 2021, 07:32:35
 * @Author S.Deckers
 * @Description Handles Businesslogic where multiple resources are involved
 */

namespace PXS.IfRF.BusinessLogic
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using Microsoft.AspNet.OData;
    using Microsoft.AspNetCore.Http;
    using PXS.IfRF.Data.Model;
    using PXS.IfRF.ErrorHandling;
    using PXS.IfRF.Logging;    
    using System.Collections.Generic;    
	using PXS.IfRF.Supporting;
    using d = System.Diagnostics.Debug;
    #endregion

    public class ResourceLogic : BusinessLogic
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ResourceLogic(ModelContext ifrfContext, ILoggerManager logger, IHttpContextAccessor httpContextAccessor) : base(ifrfContext, logger)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override BusinessLogicResult AfterCreate(SpecificResource specificResource)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            HttpContext context = _httpContextAccessor.HttpContext;
			//context.Dump( );
			context.Request.Headers.Dump();

            Resource resource = specificResource as Resource;
            // --- For now, set the operational status to 'O' (20210805 SDE)
            resource.OperationalStatus = "O";

			string user = context.Request.Headers.getUsername( );

            resource.SetCreated	( user, DateTime.Now);
            resource.SetModified( user, DateTime.Now);

            return new BusinessLogicResult();
        }

        public override BusinessLogicResult AfterUpdate(SpecificResource before, SpecificResource after, IEnumerable<string> changedFields)
        {
            Resource rbefore = before as Resource;
            Resource rafter = after as Resource;
            BusinessLogicResult result = new BusinessLogicResult();

            switch (rbefore.Type)
            {
                case nameof(Connection):
                    return (new ConnectionLogic(_ifrfContext, _logger)).AfterUpdateConnectionResource(rbefore, rafter, changedFields);
                default:
                    return result;
            }
        }

        public override BusinessLogicResult AfterDelete(SpecificResource resource)
        {
            throw new System.NotImplementedException();
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
    }
}
