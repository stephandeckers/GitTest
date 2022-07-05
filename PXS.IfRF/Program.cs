/**
 * @Name Program.cs
 * @Purpose 
 * @Date 26 May 2021, 13:09:29
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF
{
    #region -- Using directives --
    using System;
    using System.IO;
    using System.Net;
    using System.Collections;
	using System.Net.Security;
    using System.Threading.Tasks;
	using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Https;
    using Microsoft.Extensions.Hosting;
    using PXS.IfRF.Logging;
    using PXS.IfRF.AuthHandling;
    //using PXS.IfRF.AuthHandling.Certificates;
    using d=System.Diagnostics.Debug;
    #endregion

    static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					webBuilder.UseStartup<Startup>( );
					webBuilder.ConfigureKestrel (o => 
					{ 
						o.ConfigureHttpsDefaults(o => o.ClientCertificateMode = ClientCertificateMode.RequireCertificate);	 // --- Require certificate
					});
				 } );

        public static IHostBuilder CreateHostBuilder1(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    
                    webBuilder.UseStartup<Startup>();
                    Console.WriteLine("Configuring web host defaults, after startup");

                    // Below code is to support passthrough termination in OpenShift. Currently using Edge termination.
                    // Passthrough would be required if we need to go for 2-way certificate authentication

                    // add the certificate from the place as defined in env variables

                    IDictionary _envVar = Environment.GetEnvironmentVariables();

                    string serverCertificatePath        = _envVar["ServerCertificatePath"].ToString();
                    string serverCertificatePassword    = _envVar["ServerCertificatePassword"].ToString();

                    ConfigureKestrel( webBuilder, serverCertificatePath, serverCertificatePassword);

                });

        private static void ConfigureKestrel(IWebHostBuilder webBuilder, string serverCertPath, string serverCertPwd)
        {
            _ = webBuilder.UseKestrel(options =>
            {
                Console.WriteLine("Using Kestrel");
                var x509Cert = new X509Certificate2(File.ReadAllBytes(serverCertPath), serverCertPwd);
                Console.WriteLine($"Subject: {x509Cert.Subject}");
                Console.WriteLine($"Issuer: {x509Cert.Issuer}");
                Console.WriteLine($"IssuerName: {x509Cert.IssuerName}");
                Console.WriteLine($"Subject: {x509Cert.Subject}");
                Console.WriteLine($"SubjectName: {x509Cert.SubjectName}");

                options.Listen(IPAddress.Any, 8080, listenOptions =>
                {
                    _ = listenOptions.UseHttps((a, b, c, d) =>
                    {
                        var serverChain = new X509Certificate2Collection();
                        serverChain.Import(serverCertPath, serverCertPwd);
                        return new ValueTask<SslServerAuthenticationOptions>(new SslServerAuthenticationOptions
                        {
                            ServerCertificateContext = SslStreamCertificateContext.Create(x509Cert, serverChain),
                            // require a client certificate, note that with UseHttps the ConfigureWebHostDefaults is ignored
                            ClientCertificateRequired = true
                           //,    RemoteCertificateValidationCallback = CertificateValidator.ValidateCertificate
                        });
                    },
                    null);
                });

            });
        }
    }
}
