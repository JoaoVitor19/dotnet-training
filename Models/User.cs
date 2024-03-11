using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace dotnet_training.Models
{
    [Index(nameof(Id), nameof(Email))]
    public class User : ISoftDelete, IChanges
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User(string username, string password, string email, string role, bool isDeleted)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            IsDeleted = isDeleted;
        }
    }

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
                .NotNull().WithMessage("Email is null")
                .NotEmpty().WithMessage("Email is empty")
                .MaximumLength(200).WithMessage("Maximum length of Email is 200 characters")
                .EmailAddress().WithMessage("Email format is not valid");

            RuleFor(user => user.Username)
                .NotNull().WithMessage("Username is null")
                .NotEmpty().WithMessage("Username is empty")
                .MinimumLength(3).WithMessage("Minimum length of Username is 3 characters")
                .MaximumLength(50).WithMessage("Maximum length of Username is 50 characters");

            RuleFor(user => user.Password)
                .NotNull().WithMessage("Password is null")
                .NotEmpty().WithMessage("Password is empty")
                .MinimumLength(6).WithMessage("Minimum length of Password is 6 characters")
                .MaximumLength(200).WithMessage("Maximum length of Password is 200 characters")

                // ***** I used Must in this validation but have "Custom" for validate; *****

                .Must(password =>
                {
                    // Regular expression pattern to check for at least one uppercase character
                    string pattern = @"^(?=.*[A-Z]).+$";

                    // Use Regex.IsMatch to check if the password matches the pattern
                    return Regex.IsMatch(password, pattern);
                }).WithMessage("One uppercase character is needed");
            //.Custom((password, context) =>
            //{
            //    // Regular expression pattern to check for at least one uppercase character
            //    string pattern = @"^(?=.*[A-Z]).+$";

            //    // Use Regex.IsMatch to check if the password matches the pattern
            //    bool haveOneUpperLetter = Regex.IsMatch(password, pattern);

            //    // Add failure if not have uppercase character
            //    if (haveOneUpperLetter is false)
            //    {
            //        context.AddFailure("One uppercase character is needed");
            //    }
            //});
        }
    }
}
