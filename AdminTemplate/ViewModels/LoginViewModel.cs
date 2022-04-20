using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Your password must be min. 6 charachters long!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
