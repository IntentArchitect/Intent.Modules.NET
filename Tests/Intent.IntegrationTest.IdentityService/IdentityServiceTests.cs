using CleanArchitecture.IdentityService.Application.Interfaces;
using CleanArchitecture.IdentityService.Domain.Entities;
using CleanArchitecture.IdentityService.Infrastructure.Persistence;
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
            
            var identityServiceHost = await IdentityServiceHost.SetupIdentityService(_emailSender);

            var identityServiceManager = identityServiceHost.Services.GetService<IIdentityServiceManager>();

            _emailSender.Triggered = (confirmationLink) => { };
            await identityServiceManager.Register(new CleanArchitecture.IdentityService.Application.Identity.RegisterRequestDto
            {
                Email = "string@string.com",
                Password = "Password@123",
            });

            var dbContext = identityServiceHost.Services.GetService<ApplicationDbContext>();
            Assert.True(dbContext.Users.Count() > 0);
        }
    }

    internal class FakeEmailSender : CleanArchitecture.IdentityService.Application.Interfaces.IEmailSender<ApplicationIdentityUser>
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