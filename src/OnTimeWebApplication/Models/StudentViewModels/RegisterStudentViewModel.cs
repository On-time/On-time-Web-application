﻿using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models.StudentViewModels
{
    public class RegisterStudentViewModel
    {
        [Required]
        [Display(Name = "Student Id")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(30, ErrorMessage = "The {0} must not exceed {1} characters long.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(30, ErrorMessage = "The {0} must not exceed {1} characters long.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
