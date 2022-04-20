using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name="Username")]
        [Required(ErrorMessage="Username is required!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required!")]
        [Display(Name = "Surname")]
        [StringLength(50)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100,MinimumLength =6, ErrorMessage = "Your password must be min. 6 charachters long!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirm is required!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password confirm")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match!")]
        public string ConfirmPassword { get; set; }

    }
}
