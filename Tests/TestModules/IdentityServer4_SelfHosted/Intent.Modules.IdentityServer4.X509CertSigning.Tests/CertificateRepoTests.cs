using IdentityServer4StandaloneApi;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Tests
{
    public class CertificateRepoTests
    {
        // This test assumes that 'localhost' exists in your cert store

        [Fact(Skip = "Fails on the ubuntu build agent")]
        //System.Security.Cryptography.CryptographicException : Unix LocalMachine X509Store is limited to the Root and CertificateAuthority stores.
        //at Internal.Cryptography.Pal.StorePal.FromSystemStore(String storeName, StoreLocation storeLocation, OpenFlags openFlags)
        //at System.Security.Cryptography.X509Certificates.X509Store.Open(OpenFlags flags)
        //at IdentityServer4StandaloneApi.CertificateRepo.GetFromCertificateStore(X509FindType findType, String findValue, StoreName storeName, StoreLocation storeLocation) in /home/vsts/work/1/s/Tests/TestModules/IdentityServer4_SelfHosted/IdentityServer4StandaloneApi/CertificateRepo.cs:line 42
        //at Intent.Modules.IdentityServer4.X509CertSigning.Tests.CertificateRepoTests.GetLocalhostCertificateFromCertStore_FindTypeEnum() in /home/vsts/work/1/s/Tests/TestModules/IdentityServer4_SelfHosted/Intent.Modules.IdentityServer4.X509CertSigning.Tests/CertificateRepoTests.cs:line 25
        //----- Inner Stack Trace -----
        public void GetLocalhostCertificateFromCertStore_FindTypeString()
        {
            var cert = CertificateRepo.GetFromCertificateStore("FindBySubjectName", "localhost");
            Assert.NotNull(cert);
            Assert.Equal("CN=localhost", cert.Subject);
        }

        [Fact(Skip = "Fails on the ubuntu build agent")]
        public void GetLocalhostCertificateFromCertStore_FindTypeEnum()
        {
            var cert = CertificateRepo.GetFromCertificateStore(X509FindType.FindBySubjectName, "localhost");
            Assert.NotNull(cert);
            Assert.Equal("CN=localhost", cert.Subject);
        }
    }
}
