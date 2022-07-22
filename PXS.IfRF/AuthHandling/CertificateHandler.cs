using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using PXS.IfRF.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PXS.IfRF.AuthHandling
{
    public static class CertificateHandler
    {
        public static bool ValidateCertificate(object o, X509Certificate cert, X509Chain clientChain, SslPolicyErrors err)
        {
            Console.WriteLine($"ValidateCertificate running:{System.DateTime.Now}");

            // Validate the certificate chain
            if (err != SslPolicyErrors.None)
            {
                //Console.WriteLine($"SSl policy error: {err} for certificate: {cert}");
                Console.WriteLine($"ValidateCertificate:SSl policy error: [{err}]");

                if( cert == null)
                {
                    Console.WriteLine($"ValidateCertificate:SSl policy error:cert is null");
                }
                else
                {
                    Console.WriteLine($"ValidateCertificate:SSl policy error:cert [{err}]");
                }

                if( clientChain == null)
                {
                    Console.WriteLine($"ValidateCertificate:SSl policy error:clientChain is null");
                   // return( false); // allow connection without client cert -> Doesn't work (20220721 SDE)
                }

                if( clientChain != null)
                {
                    foreach (X509ChainStatus status in clientChain?.ChainStatus)
                    {
                        Console.WriteLine($"ValidateCertificate:Status: {status.Status}, information: {status.StatusInformation}");
                    }
                }

                Console.WriteLine($"ValidateCertificate returned false:{System.DateTime.Now}");
                return false;
            }

            // Validate that all attributes are OK
            var issuer = new Dictionary<string, string>();
            cert.Issuer.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var issuerKvp = x.Split("=");
                string key = issuerKvp[0];
                string value = issuerKvp[1];
                if (!issuer.ContainsKey(key))
                {
                    issuer.Add(key, value);
                }
            });

            Console.WriteLine($"ValidateCertificate: invoking ValidateIssuer:{System.DateTime.Now}");

            bool result = ValidateIssuer(issuer);

            Console.WriteLine($"ValidateCertificate returned {result}:{System.DateTime.Now}");
            return (result);
        }

        public static X509Certificate2 ReadServerCertificate(string serverCertPath, string serverCertPwd)
        {
            var x509Cert = new X509Certificate2(File.ReadAllBytes(serverCertPath), serverCertPwd);
            Console.WriteLine($"  ReadServerCertificate:Subject: {x509Cert.Subject}");
            Console.WriteLine($"  ReadServerCertificate:Issuer: {x509Cert.Issuer}");
            Console.WriteLine($"  ReadServerCertificate:IssuerName: {x509Cert.IssuerName}");
            Console.WriteLine($"  ReadServerCertificate:Subject: {x509Cert.Subject}");
            Console.WriteLine($"  ReadServerCertificate:SubjectName: {x509Cert.SubjectName}");
            return x509Cert;
        }

        public static X509Certificate2Collection AddCertificateChainToStores(string serverCertPath, string serverCertPwd)
        {
            var serverChain = new X509Certificate2Collection();
            serverChain.Import(serverCertPath, serverCertPwd);
            Console.WriteLine($"  AddCertificateChainToStores:Server chain has {serverChain.Count} certificates");

            RegisterCertificate("root", StoreName.Root, serverChain);

            RegisterCertificate("issuing", StoreName.CertificateAuthority, serverChain);

            return serverChain;
        }

        private static void RegisterCertificate(string certType, StoreName storeName, X509Certificate2Collection serverChain)
        {
            string storeNameString = storeName.ToString();

            using var store = new X509Store(storeName, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection storeCertificates = store.Certificates;
            Console.WriteLine($"    RegisterCertificate:{storeNameString} store has {storeCertificates.Count} certificates");
            Console.WriteLine("     RegisterCertificate:Listing certificates: ");
            foreach (X509Certificate2 aCertificate in storeCertificates)
            {
                Console.WriteLine(aCertificate.Subject);
            }
            X509Certificate2Collection foundCerts = serverChain.Find(X509FindType.FindBySubjectName, certType, false);
            Console.WriteLine($"    RegisterCertificate:{certType} certs found: {foundCerts.Count}");
            X509Certificate2 certToAdd = serverChain[0];
            if (foundCerts.Count > 0)
            {
                certToAdd = foundCerts[0];
            }
            bool foundCert = storeCertificates.Find(X509FindType.FindBySerialNumber, certToAdd.SerialNumber, true).Count > 0;
            if (!foundCert)
            {
                Console.WriteLine($"    RegisterCertificate:Adding {certType} certificate");
                store.Add(certToAdd);
            }
        }

        /*
         Private methods
         */
        private static bool ValidateIssuer
        (
            Dictionary<string, string> issuer
        )
        {
            string issuerCN = issuer["CN"];
            string issuerOrganisation = issuer["O"].ToUpper();

            if (!issuerCN.Equals("ProximusCorporateIssuingCA"))
            {
                return false;
            }

            if (!(issuerOrganisation.Contains("PROXIMUS") || issuerOrganisation.Contains("BELGACOM")))
            {
                return false;
            }
            return true;
        }
    }
}
