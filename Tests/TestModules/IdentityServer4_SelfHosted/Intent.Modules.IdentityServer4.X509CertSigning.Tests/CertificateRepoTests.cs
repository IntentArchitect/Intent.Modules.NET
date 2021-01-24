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

        [WindowsOnlyFact]
        public void GetLocalhostCertificateFromCertStore_FindTypeString()
        {
            var cert = CertificateRepo.GetFromCertificateStore("FindBySubjectName", "localhost");
            Assert.NotNull(cert);
            Assert.Equal("CN=localhost", cert.Subject);
        }

        [WindowsOnlyFact]
        public void GetLocalhostCertificateFromCertStore_FindTypeEnum()
        {
            var cert = CertificateRepo.GetFromCertificateStore(X509FindType.FindBySubjectName, "localhost");
            Assert.NotNull(cert);
            Assert.Equal("CN=localhost", cert.Subject);
        }
    }
}
