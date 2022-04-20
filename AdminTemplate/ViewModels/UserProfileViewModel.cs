using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.ViewModels
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Name is required!")]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required!")]
        [Display(Name = "Surname")]
        [StringLength(50)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-mail is required!")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
