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
    using System.Linq;
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
	using System.Diagnostics;
	#endregion

	static class Program
    {
        public static void Main(string[] args)
        {
            string procName = Process.GetCurrentProcess().ProcessName;

			if( procName == "iisexpress")
			{
				CreateIISHostBuilder( args ).Build( ).Run( );
				return;
			}

            CreateKestrelHostBuilder(args).Build().Run();
        }

		public static IHostBuilder CreateIISHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );

		public static IHostBuilder CreateKestrelHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );

		public static IHostBuilder CreateKestrelHostBuilder1(string[] args) =>
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

                    Console.WriteLine( $"CreateHostBuilder:serverCertificatePath=[{serverCertificatePath}]");
                    Console.WriteLine( $"CreateHostBuilder:serverCertificatePassword read by environment");

                    ConfigureKestrel( webBuilder, serverCertificatePath, serverCertificatePassword);
                });

        private static void ConfigureKestrel(IWebHostBuilder webBuilder, string serverCertPath, string serverCertPwd)
        {
            Console.WriteLine( $"ConfigureKestrel running");

            _ = webBuilder.UseKestrel(options =>
            {
                X509Certificate2            x509Cert    = CertificateHandler.ReadServerCertificate      ( serverCertPath, serverCertPwd);
                X509Certificate2Collection  serverChain = CertificateHandler.AddCertificateChainToStores( serverCertPath, serverCertPwd);

                int port = 8080;
                //port = 443; // --- Doesn't start, no permission (20220707 SDE)
                options.Listen(IPAddress.Any, port, listenOptions =>
                {
                    _ = listenOptions.UseHttps((a, b, c, d) =>
                    {

                        return new ValueTask<SslServerAuthenticationOptions>(new SslServerAuthenticationOptions
                        {
                            ServerCertificateContext = SslStreamCertificateContext.Create(x509Cert, serverChain),
                            // require a client certificate, note that with UseHttps the ConfigureWebHostDefaults is ignored
                            ClientCertificateRequired = true,
                            //ClientCertificateRequired = false,
                            RemoteCertificateValidationCallback = CertificateHandler.ValidateCertificate
                        });
                    },
                    null);
                });

            });
        }
    }
}
