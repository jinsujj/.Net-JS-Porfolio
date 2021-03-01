using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyApp.Models.DotNetNote;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Data.Repositorys.DotNetNote
{
    public class NoteRepository : INoteRepository
    {
        private IConfiguration _config;
        private MySqlConnection con;
        private ILogger<NoteRepository> _logger;

        public NoteRepository(IConfiguration config, ILogger<NoteRepository> logger)
        {
            _config = config;
            con = new MySqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            _logger = logger;
        }

        public void Add(Note model)
        {
            _logger.LogInformation("노트 작성");
            try
            {
                SaveOrUpdate(model, BoardWriteFormType.Write);
            }
            catch (Exception ex)
            {
                _logger.LogError("노트 자성 에러: " + ex);
            }
        }

        public int UpdateNote(Note model)
        {
            int r = 0;
            _logger.LogInformation("노트 수정");
            try
            {
                r = SaveOrUpdate(model, BoardWriteFormType.Modify);
            }
            catch (Exception ex)
            {
                _logger.LogError("노트 수정 에러: " + ex);
            }
            return r;
        }

        public void ReplyNote(Note model)
        {
            _logger.LogInformation("노트 답변");
            try
            {
                SaveOrUpdate(model, BoardWriteFormType.Reply);
            }
            catch (Exception ex)
            {
                _logger.LogError("노트 답변 에러: " + ex);
            }
        }

        public int SaveOrUpdate(Note n, BoardWriteFormType formType)
        {
            int r = 0;
            var p = new DynamicParameters();

            p.Add("@Name", value: n.Name, dbType: DbType.String);
            p.Add("@Email", value: n.Email, dbType: DbType.String);
            p.Add("@HomePage", value: n.HomePage, dbType: DbType.String);
            p.Add("@Title", value: n.Title, dbType: DbType.String);
            p.Add("@Content", value: n.Content, dbType: DbType.String);
            p.Add("@Password", value: n.Password, dbType: DbType.String);
            p.Add("@Encoding", value: n.Encoding, dbType: DbType.String);
            p.Add("@FileName", value: n.FileName, dbType: DbType.String);
            p.Add("@FileSize", value: n.FileSize, dbType: DbType.Int32);
            p.Add("@Category", value: n.Category, dbType: DbType.String);
            switch (formType)
            {
                case BoardWriteFormType.Write:
                    p.Add("@PostIp", value: n.PostIp, dbType: DbType.String);
                    string Ref = con.Query<string>(@"SELECT MAX(ref) FROM notes").SingleOrDefault();

                    if (Ref == null)
                        p.Add("@maxRef", value: 1, dbType: DbType.Int32);
                    else
                        p.Add("@maxRef", value: Convert.ToInt32(Ref) + 1, dbType: DbType.Int32);

                    string Write_S001 = @"INSERT INTO notes (Name, Email, Title, PostIp, Content, Password, Encoding, HomePage ,Ref ,FileName , FileSize, Category)
                                       VALUES(@Name, @Email, @Title, @PostIp, @Content, @Password, @Encoding, @HomePage, @maxRef, @FileName, @FileSize, @Category )";
                    r = con.Execute(Write_S001, p, commandType: CommandType.Text);
                    break;



                case BoardWriteFormType.Modify:
                    p.Add("ModifyIp", value: n.ModifyIp, dbType: DbType.String);
                    p.Add("@Id", value: n.Id, dbType: DbType.Int32);

                    // [1] 번호와 암호 확인 
                    string Modify_S001 = @"SELECT COUNT(*) 
                                          FROM notes
                                          WHERE Id = @Id
                                          AND Password = @Password";
                    int cnt = con.Query<int>(Modify_S001, new { Id = n.Id, Password = n.Password }).SingleOrDefault();

                    if (cnt > 0)
                    {
                        string Modify_U001 = @"UPDATE notes
                                               SET Name = @Name, Email = @Email, Title = @Title, ModifyIp = @ModifyIp, ModifyDate = NOW(), Content = @Content, Encoding = @Encoding
                                                   ,HomePage = @HomePage, FileName = @FileName, FileSize = @FileSize
                                               WHERE Id = @Id";
                        con.Execute(Modify_U001, p);
                        r = 1;
                    }
                    else
                        r = 0;
                    break;



                case BoardWriteFormType.Reply:
                    int maxRefOrder, maxRefAnswerNum, parentRef, parentStep;
                    // [1] 부모글의 답변수 1 증가 
                    string Reply_U001 = @"UPDATE notes SET AnswerNum = AnswerNum +1 
                                   WHERE Id = @ParentNum";

                    con.Execute(Reply_U001, new { ParentNum = n.ParentNum }, commandType: CommandType.Text);

                    // [2] 같은 글에 대해서 답변을 2번 이상하면 먼저 답변한게 위에 나타나게 한다.
                    string Reply_S001 = @"SELECT RefOrder, AnswerNum
                                    FROM notes
                                    WHERE ParentNum = @ParentNum 
                                    AND RefOrder = (SELECT MAX(RefOrder)
                                                    FROM notes
                                                    WHERE ParentNum = @ParentNum)";

                    var results = con.Query(Reply_S001, new { ParentNum = n.ParentNum }).SingleOrDefault();
                    // [2-1] 답변이 없을 경우 
                    if (results == null)
                    {
                        Reply_S001 = @"SELECT RefOrder
                                       FROM notes
                                       WHERE Id = @ParentNum";
                        var temp = con.Query(Reply_S001, new { ParentNum = n.ParentNum }).SingleOrDefault();

                        maxRefOrder = temp.RefOrder;
                        maxRefAnswerNum = 0;
                    }
                    else
                    {
                        maxRefOrder = results.RefOrder;
                        maxRefAnswerNum = results.AnswerNum;
                    }

                    // [3] 중간에 답변달 때 (비집고 들어갈 자리 마련)
                    Reply_S001 = @"SELECT Ref, Step
                                       FROM notes
                                       WHERE Id = @ParentNum";
                    results = con.Query(Reply_S001, new { ParentNum = n.ParentNum }).SingleOrDefault();
                    // [3-1] 답변이 없을 경우
                    if (results == null)
                    {
                        parentRef = 0;
                        parentStep = 0;
                    }
                    else
                    {
                        parentRef = results.Ref;
                        parentStep = results.Step;
                    }

                    Reply_U001 = @"UPDATE notes SET RefOrder = RefOrder +1
                                   WHERE Ref = @ParentRef 
                                   AND RefOrder > (@MaxRefOrder + @MaxRefAnswerNum)";
                    con.Execute(Reply_U001, new { ParentRef = parentRef, MaxRefOrder = maxRefOrder, MaxRefAnswerNum = maxRefAnswerNum });

                    // [4] 최종 저장
                    p.Add("@PostIp", value: n.PostIp, dbType: DbType.String);
                    p.Add("@ParentNum", value: n.ParentNum, dbType: DbType.Int32);
                    p.Add("@ParentRef", value: parentRef, dbType: DbType.Int32);
                    p.Add("@ParentStep", value: parentStep, dbType: DbType.Int32);
                    p.Add("@MaxRefOrder", value: maxRefOrder, dbType: DbType.Int32);
                    p.Add("@MaxRefAnswerNum", value: maxRefAnswerNum, dbType: DbType.Int32);
                    p.Add("@Category", value: n.Category, dbType: DbType.String);

                    string Reply_I001 = @"INSERT notes (Name, Email, Title, PostIp, Content, Password, Encoding, HomePage, Ref, Step, RefOrder, ParentNum, FileName, FileSize, Category)
                                          VALUES (@Name, @Email, @Title, @PostIp, @Content, @Password, @Encoding, @HomePage, @ParentRef, @ParentStep +1,
                                                  @MaxRefOrder + @MaxRefAnswerNum +1, @ParentNum, @FileName, @FileSize, @Category)";
                    con.Execute(Reply_I001, p);
                    break;
            }
            return r;
        }

        public int DeleteNote(int id, string password)
        {
            _logger.LogInformation("노트 삭제");
            string sql = @"SELECT COUNT(*) FROM notes
                          WHERE id = @id
                          AND password = @password";
            int cnt = con.Query<int>(sql, new { id = id, password = password }).SingleOrDefault();

            if (cnt == 0)
            {
                return 0;
            }

            try
            {
                sql = @"SELECT answernum, reforder, Ref, parentnum
                        FROM notes
                        WHERE id =@id";
                var results = con.Query(sql, new { id = id }).SingleOrDefault();

                int answerNum = results.answernum;
                int refOrder = results.reforder;
                int Ref = results.Ref;
                int parentNum = results.parentnum;

                //if (results.answernum == 0)
                //{
                    sql = @"DELETE FROM notes WHERE id = @id";
                    con.Execute(sql, new { id = id });

                    sql = @"DELETE FROM notes
                                WHERE id = @parentnum
                                AND modifyip = '((DELETED))'
                                AND AnswerNum = 0";

                    con.Execute(sql, new { id = parentNum });
                //}
                //else
                //{
                //    sql = @"UPDATE notes
                //                SET reforder = reforder -1
                //                WHERE Ref  = @Ref
                //                AND reforder > @reforder";
                //    con.Execute(sql, new { Ref = Ref, reforder = refOrder });
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError("노트 삭제 에러" + ex);
            }
            return 1;
        }

        public List<Note> GetAll(int page, string category)
        {
            _logger.LogInformation("노트 데이터 조회");
            try
            {
                var parameters = new DynamicParameters(new { Page = page , category = category});
                string sql = @"WITH DotNetNoteOrderedLists 
                               AS(  
                                    SELECT Id, Name, Email, Title, PostDate, ReadCount, Ref, Step, RefOrder
                                        ,AnswerNum, ParentNum, CommentCount, FileName, FileSize, DownCount
                                        , Content
                                        , ROW_NUMBER() OVER (ORDER BY Ref DESC, RefOrder ASC) AS RowNumber
                                    FROM notes
                                    WHERE category LIKE @category
                               )
                               SELECT * FROM DotNetNoteOrderedLists
                               WHERE RowNumber BETWEEN @Page *5 + 1 AND (@Page +1) * 5";
                return con.Query<Note>(sql, parameters, commandType: CommandType.Text).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("노트 데이터 출력 에러" + ex);
                return null;
            }
        }

        public List<Note> GetCardAll(string category)
        {
            _logger.LogInformation("카드 목록 조회");
            try
            {
                string sql = @"SELECT *
                               FROM notes 
                               WHERE parentnum = 0
                               AND category LIKE @category
                               ORDER BY postdate DESC";
                return con.Query<Note>(sql, new { category = category }, commandType: CommandType.Text).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError("노트 카드 출력 에러 " + ex);
                return null;
            }
        }

        public int GetCountAll()
        {
            _logger.LogInformation("전체 게시글 조회");
            try
            {
                return con.Query<int>(@"SELECT COUNT(*) FROM notes").SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("게시글 조회 에러" + ex);
                return -1;
            }
        }

        public int GetCountBySearch(string searchField, string searchQuery)
        {
            try
            {
                string sql = @"SELECT COUNT(*)
                           FROM notes
                           WHERE ( CASE @searchField
                                   WHEN 'Name' THEN Name
                                   WHEN 'Title' THEN Title
                                   WHEN 'Content' THEN Content
                                   ELSE @searchQuery END)
                           LIKE CONCAT('%', @searchQuery, '%')";
                return con.Query<int>(sql, new { SearchQuery = searchQuery, SearchField = searchField }).SingleOrDefault();

            }
            catch (Exception ex)
            {
                _logger.LogError("검색 카운트 출력 에러" + ex);
                return -1;
            }
        }

        public string GetFileNameById(int id)
        {
            return con.Query<string>(
               @"SELECT filename FROM notes WHERE Id = @id ;",
               new { Id = id }).SingleOrDefault();
        }

        public List<Note> GetNewPhotos()
        {
            string sql =
                @"SELECT Id, Title, FileName, FileSize
                  FROM notes
                  WHERE FileName LIKE CONCAT('%','.png') OR FileName LIKE CONCAT('%','.jpg')
                  OR FileName LIKE CONCAT('%','.jpeg') OR FileName Like CONCAT('%','.gif')
                  ORDER BY Id DESC
				  LIMIT 5;";
            return con.Query<Note>(sql).ToList();
        }

        public string GetLatestId(int page, string category)
        {
            string sql = "";
            try
            {
                if (page == 0)
                {
                     sql = @"SELECT id 
                            FROM notes 
                            WHERE category LIKE @category
                            ORDER BY id desc";
                    return con.Query<string>(sql, new { category = category }).First();
                }
                else
                {
                    sql = @"SELECT id
                            FROM (
	                            SELECT id, ROW_NUMBER() OVER( ORDER BY ref DESC, reforder ASC ) AS ROWNUMBER
	                            FROM notes
                            ) A
                            WHERE A.ROWNUMBER BETWEEN @Page * 5 +1  AND (@Page + 1) *5
                            ORDER BY ID DESC";
                    return con.Query<string>(sql, new { category = category, page = page }).First();
                }
            }
            catch (Exception ex)
            {
                return "1";
            }
        }

        public Note GetNoteById(int id)
        {
            var parameters = new DynamicParameters(new { Id = id });
            string U001 = @"UPDATE notes
                           SET ReadCount = ReadCount +1 
                           WHERE Id = @id";

            string S001 = @"SELECT * FROM notes
                            WHERE Id = @id";

            con.Execute(U001, parameters, commandType: CommandType.Text);
            return con.Query<Note>(S001, parameters, commandType: CommandType.Text).SingleOrDefault();
        }

        /// <summary>
        /// 최근 글 리스트 : Home\Index.cshtml 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<Note> GetNoteSummaryByCategory(string category)
        {
            string sql =
                @"SELECT Id, Title, Name, PostDate, FileName, 
                         ReadCount, CommentCount, Step
                  FROM notes
                  WHERE category LIKE @category 
                  ORDER BY Id DESC
                  LIMIT 3";
            return con.Query<Note>(sql, new { category = category }).ToList();
        }

        public List<Note> GetRecentPosts(int numberOfNotes)
        {
            string sql =
                @"SELECT {numberOfNotes} Id, Title, Name, PostDate
                   FROM notes 
                   ORDER BY Id DESC";
            return con.Query<Note>(sql).ToList();
        }

        public List<Note> GetSearchAll(int page, string searchField, string searchQuery)
        {
            var parameters = new DynamicParameters(new { Page = page, SearchFiled = searchField, SearchQuery = searchQuery });
            string sql = @"SELECT Id, Name, Email, Title, PostDate, ReadCount, Ref, Step, RefOrder, AnswerNum,
                                        ParentNum, CommentCount, FileName, FileSize, DownCount, RowNumber
                             FROM (
                                SELECT Id, Name, Email, Title, PostDate, ReadCount, Ref, Step, RefOrder, AnswerNum,
                                        ParentNum, CommentCount, FileName, FileSize, DownCount,
                                        ROW_NUMBER() OVER (ORDER BY Ref DESC, RefOrder ASC) AS RowNumber
                                FROM notes
                                WHERE (
                                        CASE @SearchFiled
                                            WHEN 'Name' Then Name
                                            WHEN 'Title' Then Title
                                            WHEN 'Content' THEN Content
                                            ELSE @SearchQuery   END
                                      ) LIKE CONCAT('%', @SearchQuery, '%')
                             ) a
                             WHERE RowNumber BETWEEN @Page *5 +1 AND (@Page +1 ) * 5
                             ORDER BY Id DESC";
            return con.Query<Note>(sql, parameters).ToList();
        }

        public void Log(string page, string ip)
        {
            con.Execute(@"INSERT INTO log SET page = @page, ip = @ip, date = NOW()"
            , new { page= page, ip = ip });
        }

        public void Pinned(int id)
        {
            con.Execute(
                @"UPDATE notes SET Category = 'Notice'
                   WHERE Id = @Id"
                , new { Id = id });
        }

        public void UpdateDownCount(string fileName)
        {
            con.Execute(
                @"UPDATE notes SET DownCount = DownCount +1
                  WHERE FileName = @FileName"
                , new { FileName = fileName });
        }
        public void UpdateDownCountById(int id)
        {
            var p = new DynamicParameters(new { Id = id });
            con.Execute(
                @"UPDATE notes SET DownCount = DownCount +1
                  WHERE Id = @Id"
                , p);
        }
    }
}
