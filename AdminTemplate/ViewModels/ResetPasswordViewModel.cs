using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "New password is required!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Your password must be min. 6 charachters long!")]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match!")]
        public string ConfirmNewPassword { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
    }
}
