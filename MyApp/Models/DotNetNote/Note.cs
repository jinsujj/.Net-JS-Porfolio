using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Models.DotNetNote
{
    public class Note
    {
        [Display(Name = "번호")]
        public int Id { get; set; }

        [Required(ErrorMessage = "* 내용을 작성해 주세요")]
        [Display(Name = "작성자")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "이메일을 양식에 맞춰 입력해주세요")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "* 내용을 작성해 주세요")]
        [Display(Name = "제목")]
        public string Title { get; set; }

        [Display(Name = "작성일")]
        public DateTime PostDate { get; set; }
        public string PostDates { get; set; }
        public string PostIp { get; set; }
        [Display(Name = "내용")]
        [Required(ErrorMessage = "* 내용을 작성해 주세요")]
        public string Content { get; set; }
        [Display(Name = "비밀번호")]
        [Required(ErrorMessage = "* 비밀번호를 입력해 주세요")]
        public string Password { get; set; }
        [Display(Name = "조회수")]
        public int ReadCount { get; set; }
        [Display(Name = "인코딩")]
        public string Encoding { get; set; } = "Text";
        public string HomePage { get; set; } = "";
        public DateTime ModifyDate { get; set; }
        public string ModifyIp { get; set; }
        [Display(Name = "파일")]
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public int DownCount { get; set; }
        public int Ref { get; set; }
        public int Step { get; set; }
        public int RefOrder { get; set; }
        public int AnswerNum { get; set; }
        public int ParentNum { get; set; }
        public int CommentCount { get; set; }
        public string Category { get; set; } = "Free";
        public bool isMain { get; set; } = false;
    }
}
