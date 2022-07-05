using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PXS.IfRF.Logging;
using PXS.IfRF.Services;
using PXS.IfRF.Supporting;
using PXS.IfRF.Controllers;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.AuthHandling
{
	public class WebsealAuthenticationOptions
        : AuthenticationSchemeOptions
    {}

	public class CertAndWebsealAuthenticationHandler
        : AuthenticationHandler<WebsealAuthenticationOptions>
    {
        readonly ILoggerManager _logger;
        public CertAndWebsealAuthenticationHandler
		(
			IOptionsMonitor<WebsealAuthenticationOptions>	options
		,	ILoggerFactory									loggerFactory
		,	UrlEncoder										encoder
		,	ISystemClock									clock
		,	ILoggerManager									logger
            )
            : base(options, loggerFactory, encoder, clock)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            _logger = logger;
        }

        private enum CertificateAuthorizationType
        {
            Invalid,
            Webseal,
            CN
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            //throw new System.NotImplementedException( "Argh");

            _logger.Debug( $"SDE:HandleAuthenticateAsync checking certificate");

            string subjectCN;
            string groupFromCN;
            CertificateAuthorizationType certType = GetCertificateAuthorization(out subjectCN, out groupFromCN);

            List<Claim> claims = new();

            if (certType == CertificateAuthorizationType.Invalid)
            {
                string failureMessage = "Invalid CN in certificate";
                _logger.Warn(failureMessage);
                return Task.FromResult(AuthenticateResult.Fail(failureMessage));
            }

            //_logger.Trace( $"subjectCN={subjectCN} / {certType.ToString()} certificate detected");

            string claimsType = string.Empty;
            if (certType == CertificateAuthorizationType.Webseal)
            {
                claimsType = "Webseal";
                _logger.Debug( $"subjectCN=[{subjectCN}], [{certType.ToString()}] certificate detected");
                AddWebsealClaims(ref claims);
            } 
            else if (certType == CertificateAuthorizationType.CN)
            {
                claimsType = "CN";
                _logger.Debug( $"groupFromCN=[{groupFromCN}], [{certType.ToString()}] certificate detected");
                AddOtherClaims(ref claims, groupFromCN);
            }

            // generate claimsIdentity on the name of the class
            var claimsIdentity = new ClaimsIdentity(claims, claimsType);

            // generate AuthenticationTicket from the Identity
            // and current authentication scheme
            var ticket = new AuthenticationTicket( new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

            // pass on the ticket to the middleware
            var res = Task.FromResult(AuthenticateResult.Success(ticket));

            return( res);
        }

        private CertificateAuthorizationType GetCertificateAuthorization
        (
            out string subjectCN
        ,   out string groupFromCN
        )
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            _logger.Debug( "SDE:Entering [GetCertificateAuthorization]");

            IDictionary     _envVar     = Environment.GetEnvironmentVariables();
            IList<string>   allowedCN   = _envVar["AllowedCN"].ToString().ToUpper().Split('|').ToList();

            subjectCN   = string.Empty;
            groupFromCN = string.Empty;

            var subject = new Dictionary<string, string>();
            var clientCert = Request.HttpContext.Connection.ClientCertificate;

            if( clientCert == null)
            {
                _logger.Debug( "SDE:Client is not using a certificate, defaulting to WebSeal");
                return( CertificateAuthorizationType.Webseal);
                //return( CertificateAuthorizationType.Invalid);
            }

            _logger.Debug( "SDE:Client is using a certificate");

            clientCert.Subject.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var     subjectKvp  = x.Split("=");
                string  key         = subjectKvp[0];
                string  value       = subjectKvp[1];
                if (subjectKvp[0].Equals("OU") && subjectKvp[1].Contains(":"))
                {
                    key = subjectKvp[0] = "OU_" + subjectKvp[1].Split(":")[0];
                    value = subjectKvp[1].Split(":")[1];
                }

                if (!subject.ContainsKey("key"))
                {
                    subject.Add(key, value);
                }
            });

            //string subjectCN = subject["CN"].ToUpper();
            subjectCN   = subject["CN"].ToUpper();
            groupFromCN = string.Empty;

            string theSubject = subjectCN;

            _logger.Debug( $"SDE:subjectCN={subjectCN}");

            _logger.Debug( $"SDE:dumping allowedCN, {allowedCN.Count} items");

            int i = 1;
            foreach( var item in allowedCN)
            {
                _logger.Debug( $"SDE:allowedCN {i++}={item}");
            }

            if (!(allowedCN.Any(cn => cn.ToUpper().Contains( theSubject))))
            {
                _logger.Debug( $"SDE:subjectCN={subjectCN} not in 'allowedCN'");
                return CertificateAuthorizationType.Invalid;
            }
			
            if (subjectCN.StartsWith("INTRA.") && subjectCN.EndsWith(".WEB.BC"))
            {
                // webseal, will use http headers to get roles
                _logger.Debug( $"SDE:subjectCN={subjectCN} 'INTRA'/'.WEB.BC'");
                return CertificateAuthorizationType.Webseal;
            }

            if (subjectCN.StartsWith("IFRF-DEV"))
            {
                // webseal, will use http headers to get roles
                 _logger.Debug( $"SDE:subjectCN={subjectCN} 'IFRF-DEV'");
                return CertificateAuthorizationType.Webseal;
            }

            // no webseal, will use first part of CN to get roles
            groupFromCN = subjectCN.Split("-")[0];
             _logger.Debug( $"SDE:groupFromCN={groupFromCN}");
            return CertificateAuthorizationType.CN;
        }

        private void AddWebsealClaims(ref List<Claim> claims)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            // create claims list from the model
            //string UserId = Request.Headers["iv-user"].ToString();
			string UserId = Request.Headers.getUsername();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, UserId));

            if (Request.Headers.ContainsKey("iv-groups"))
            {
                string ivGroupsString = Request.Headers["iv-groups"].ToString();
                string[] ivGroups = ivGroupsString.Split(",");
                foreach (string aGroup in ivGroups)
                {
                    string aGroupUnquoted = aGroup.Replace("\"", "");
                    claims.Add(new Claim(ClaimTypes.Role, aGroupUnquoted));
                }
            }
        }

        private void AddOtherClaims(ref List<Claim> claims, string groups)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//2do:Add some flexible mapping between groups and roles

			//string theRole = Controllers.IfRFBaseController.EDITORROLE_lvl_2;
			if( groups == "NEDA DIVISION")
			{
				claims.Add( new Claim( ClaimTypes.NameIdentifier,	IfRFBaseController.NEDA_GRP_001));
				claims.Add( new Claim( ClaimTypes.Role,				IfRFBaseController.NEDA_GRP_001));

                //string userId = Request.Headers["iv-user"].ToString();
				string userId = Request.Headers.getUsername();

                if( string.IsNullOrEmpty( userId))
                {
                    userId = "nedaUsr";
                }

                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
			}
        }
    }
}
