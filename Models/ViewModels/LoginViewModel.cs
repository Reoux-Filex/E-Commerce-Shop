// File: Models/ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Shop.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}