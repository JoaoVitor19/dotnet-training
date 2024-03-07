using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_training.Models
{
    public class User : ISoftDelete, IChanges
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public bool isValid()
        {
            //Apply logic for validate entity;
            return true;
        }

        public User(string username, string password, string email, string role, bool isDeleted, DateTime createdAt)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            IsDeleted = isDeleted;
            CreatedAt = createdAt;
        }
    }
}
