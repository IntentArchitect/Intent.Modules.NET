using CleanArchitecture.IdentityService.Application.Interfaces;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Intent.IntegrationTest.IdentityService
{
    public class IdentityServiceTests
    {
        private readonly FakeEmailSender _emailSender;

        public IdentityServiceTests()
        {
            _emailSender = new FakeEmailSender();
        }

        [Fact]
        public async Task TestRegisterAccount()
        {
            var identityServiceHost = await IdentityServiceHost.SetupIdentityService(_emailSender, null, 5001);
            var identityServiceManager = identityServiceHost.Services.GetService<IIdentityServiceManager>();

            _emailSender.Triggered = (confirmationLink) => { };
            await identityServiceManager.Register(new CleanArchitecture.IdentityService.Application.Identity.RegisterRequestDto
            {
                Email = "string@string1.com",
                Password = "Password@123",
            });

            var dbContext = identityServiceHost.Services.GetService<ApplicationDbContext>();
            Assert.True(dbContext.Users.Count() > 0);
        }

        [Fact]
        public async Task TestLoginAccount()
        {
            var identityServiceHost = await IdentityServiceHost.SetupIdentityService(_emailSender, null, 6001);
            var identityServiceManager = identityServiceHost.Services.GetService<IIdentityServiceManager>();

            _emailSender.Triggered = (confirmationLink) => { };
            await identityServiceManager.Register(new CleanArchitecture.IdentityService.Application.Identity.RegisterRequestDto
            {
                Email = "string@string2.com",
                Password = "Password@123",
            });

            var token = await identityServiceManager.Login(new CleanArchitecture.IdentityService.Application.Identity.LoginRequestDto
            {
                Email = "string@string2.com",
                Password = "Password@123",
            }, false, false);

            Assert.True(!string.IsNullOrWhiteSpace(token.AccessToken));
        }

        [Fact]
        public async Task TestRefreshToken()
        {
            var identityServiceHost = await IdentityServiceHost.SetupIdentityService(_emailSender, null, 7001);
            var identityServiceManager = identityServiceHost.Services.GetService<IIdentityServiceManager>();

            _emailSender.Triggered = (confirmationLink) => { };
            await identityServiceManager.Register(new CleanArchitecture.IdentityService.Application.Identity.RegisterRequestDto
            {
                Email = "string@string3.com",
                Password = "Password@123",
            });

            var token = await identityServiceManager.Login(new CleanArchitecture.IdentityService.Application.Identity.LoginRequestDto
            {
                Email = "string@string3.com",
                Password = "Password@123",
            }, false, false);

            var refreshToken = await identityServiceManager.Refresh(new CleanArchitecture.IdentityService.Application.Identity.RefreshRequestDto
            {
                RefreshToken = token.RefreshToken,
            });

            Assert.True(!string.IsNullOrWhiteSpace(token.AccessToken));
        }
    }

    internal class FakeEmailSender : CleanArchitecture.IdentityService.Application.Interfaces.IIdentityEmailSender
    {
        public Action<string> Triggered { get; set; }
        public async Task SendConfirmationLinkAsync(ApplicationIdentityUser user, string email, string confirmationLink)
        {
            Triggered(confirmationLink);
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetCodeAsync(ApplicationIdentityUser user, string email, string resetCode)
        {
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetLinkAsync(ApplicationIdentityUser user, string email, string resetLink)
        {
            await Task.CompletedTask;
        }
    }
}