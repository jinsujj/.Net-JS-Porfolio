using System.ComponentModel.DataAnnotations;

namespace MyApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "작성일 필요한 필드입니다.")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "작성일 필요한 필드입니다.")]
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "작성일 필요한 필드입니다.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [Compare("Password", ErrorMessage = "비밀번호가 서로 일치하지 않습니다.")]
        public string ConfirmPassword { get; set; }
    }
}