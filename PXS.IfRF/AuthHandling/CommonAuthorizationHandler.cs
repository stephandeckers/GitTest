using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using PXS.IfRF.Logging;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.AuthHandling
{
    internal class CommonAuthorizationHandler : AuthorizationHandler<CommonAuthorizationRequirement>
    {
        private readonly IDictionary<string, IList<string>> roleGroupMappings = new Dictionary<string, IList<string>>();

        public CommonAuthorizationHandler( IConfiguration configuration)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            var rgMaps = configuration.GetSection("RoleGroupMappings").GetChildren();
            foreach(IConfigurationSection anRgMap in rgMaps)
            {
                var children = anRgMap.GetChildren();
                string role = children.Single(c => c.Key.Equals("Role")).Value;
                var groups = children.Single(c => c.Key.Equals("Groups")).GetChildren().Select(gr => gr.Value).ToList();
                roleGroupMappings.Add(role, groups);
            }
        }

        protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, CommonAuthorizationRequirement requirement)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodBase.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                return Task.CompletedTask;
            }

            if (!roleGroupMappings.ContainsKey(requirement.RequiredRole))
            {
                throw new KeyNotFoundException($"Role {requirement.RequiredRole} not present in application configuration");
            }

            IList<string> allowedGroups = roleGroupMappings[requirement.RequiredRole];
            foreach (string anAllowedGroup in allowedGroups)
            {
                if (context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.Equals(anAllowedGroup)))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
