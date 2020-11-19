using MyApp.Models.DotNetNote;
using System.Collections.Generic;

namespace MyApp.Data.Repositorys.DotNetNote
{
    public interface INoteCommentRepository
    {
        void AddNoteComment(NoteComment model);
        int DeleteNoteComment(int boardid, int id, string password);
        int GetCountBy(int boardId, int id, string password);
        List<NoteComment> GetNoteComments(int boardId);
        List<NoteComment> GetRecentComments();
    }
}