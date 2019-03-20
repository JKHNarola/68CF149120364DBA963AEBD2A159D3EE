using System.ComponentModel.DataAnnotations;

namespace App.BL.Data.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class LoginSuccessViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public int RoleId { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Firstname is required.")]
        [MaxLength(60)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        [MaxLength(60)]
        public string LastName { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}
