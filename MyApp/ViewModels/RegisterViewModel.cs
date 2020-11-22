using System.ComponentModel.DataAnnotations;

namespace MyApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email 을 입력해 주세요")]
        [EmailAddress(ErrorMessage = "이메일 형식이 아닙니다")]
        public string Email { get; set; }


        [Required(ErrorMessage = "이름을 입력해 주세요")]
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Password를 입력해 주세요")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Password를 입력해 주세요")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "비밀번호가 서로 일치하지 않습니다.")]
        public string ConfirmPassword { get; set; }
    }
}