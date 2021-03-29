using Dapper;
using MyApp.Models.DotNetNote;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyApp.Data.Repositorys.DotNetNote
{
    public class NoteCommentRepository : INoteCommentRepository
    {
        private string _connectionString;
        private MySqlConnection con;

        public NoteCommentRepository(string connectionstring)
        {
            _connectionString = connectionstring;
            con = new MySqlConnection(_connectionString);
        }

        public void AddNoteComment(NoteComment model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@BoardId", value: model.BoardId, dbType: DbType.Int32);
            parameters.Add("@Name", value: model.Name, dbType: DbType.String);
            parameters.Add("@Opinion", value: model.Opinion, dbType: DbType.String);
            parameters.Add("@Password", value: model.Password, dbType: DbType.String);
            parameters.Add("@Ip", value: model.ip, dbType: DbType.String);

            string sql =
                @"INSERT INTO notecomments (BoardId, Name, Opinion, Password, Ip)
                  VALUES (@BoardId, @Name, @Opinion, @Password, @Ip);
                    
                  UPDATE notes SET CommentCount = CommentCount +1 
                  WHERE Id = @BoardId";

            con.Execute(sql, parameters, commandType: CommandType.Text);
        }

        public List<NoteComment> GetNoteComments(int boardId)
        {
            return con.Query<NoteComment>(
                @"SELECT id, boardid, name, opinion, DATE_FORMAT(postdate,'%y/%m/%d') as postdate FROM notecomments WHERE BoardId = @BoardId"
                , new { BoardId = boardId }
                , commandType: CommandType.Text).ToList();
        }

        public int GetCountBy(int boardId, int id, string password)
        {
            return con.Query<int>(@"SELECT COUNT(*) FROM notecomments
                WHERE BoardId = @BoardId
                AND Id = @Id 
                AND Password = @Password"
                , new { BoardId = boardId, Id = id, Password = password }
                , commandType: CommandType.Text).SingleOrDefault();
        }

        public int DeleteNoteComment(int boardid, int id, string password)
        {
            return con.Execute(@"DELETE FROM notecomments 
                WHERE BoardId = @BoardId
                AND Id = @Id
                AND Password = @Password;
                
                UPDATE notes SET CommentCount = CommentCount -1
                WHERE Id = @BoardId"
                , new { BoardId = boardid, Id = id, Password = password }
                , commandType: CommandType.Text);
        }


        public List<NoteComment> GetRecentComments()
        {
            string sql = @"SELECT Id, BoardId, Opinion, PostDate
                            FROM notecomments
                            ORDER BY Id DESC
                            LIMIT 3 ";
            return con.Query<NoteComment>(sql).ToList();
        }
    }
}
