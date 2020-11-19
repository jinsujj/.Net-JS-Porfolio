using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Models.DotNetNote
{
    public class NoteCommentViewModel
    {
        public List<NoteComment> NoteCommentList { get; set; }
        public int BoardId { get; set; }
    }
}
