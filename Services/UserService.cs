using dotnet_training.Models;

namespace dotnet_training.Services
{
    public class UserService
    {
        private List<User> Users = new List<User>()
        {
            new User { Id = 1, Username = "jao", Password = "jao", Email = "jaovitordev@gmail.com", Role = string.Empty },
            new User { Id = 2, Username = "jao1", Password = "jao1", Email = "jaovitordev1@gmail.com", Role = "developer" },
            new User { Id = 3, Username = "jao2", Password = "jao2", Email = "jaovitordev2@gmail.com", Role = "support" },
        };

        public User GetWithUsername(string username, string password)
        {
            return Users.FirstOrDefault(x => x.Username.Equals(username) && x.Password.Equals(password));
        }

        public User GetWithEmail(string email, string password)
        {
            return Users.FirstOrDefault(x => x.Email is not null && x.Email.Equals(email) && x.Password.Equals(password));
        }
    }
}
