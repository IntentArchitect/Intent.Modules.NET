using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users
{
    public class User
    {
        public User()
        {
            Username = null!;
            FirstName = null!;
            LastName = null!;
            Email = null!;
            Password = null!;
            Phone = null!;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int UserStatus { get; set; }

        public static User Create(
            int id,
            string username,
            string firstName,
            string lastName,
            string email,
            string password,
            string phone,
            int userStatus)
        {
            return new User
            {
                Id = id,
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                Phone = phone,
                UserStatus = userStatus
            };
        }
    }
}