using System;
using Microsoft.AspNetCore.Authorization;
using PXS.IfRF.Logging;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.AuthHandling
{
    internal class CommonAuthorizationRequirement : IAuthorizationRequirement
    { 
        public string RequiredRole { get; }

        public CommonAuthorizationRequirement(string requiredRole)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            RequiredRole = requiredRole;
        }
    }
}
