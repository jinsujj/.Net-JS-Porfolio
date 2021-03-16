using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Data.Repositorys.DotNetNote;
using MyApp.Models.DotNetNote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    public class DotNetNoteController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly INoteRepository _repository;
        private readonly INoteCommentRepository _commentRepository;
        private readonly ILogger<DotNetNoteController> _logger;

        public DotNetNoteController(
            IWebHostEnvironment enviorment,
            INoteRepository noteRepo,
            INoteCommentRepository commentRepo,
            ILogger<DotNetNoteController> logger)
        {
            _environment = enviorment;
            _repository = noteRepo;
            _commentRepository = commentRepo;
            _logger = logger;
        }

        public bool SearchMode { get; set; } = false;
        public string SearchField { get; set; }
        public string SearchQuery { get; set; }

        public int PageIndex { get; set; } = 0;
        public int TotalRecordCount { get; set; } = 0;

        [HttpPost]
        public async Task<string> SaveImage(IFormFile file)
        {
            string fileName = string.Empty;
            int fileSize = 0;
            string Email = User.FindFirst("Email").Value;
            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            if (file.Length > 0)
            {
                fileSize = Convert.ToInt32(file.Length);
                fileName = CommonLibrary.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.ToString()));

                using (var fileStream = new FileStream(
                       Path.Combine(uploadFolder, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            string result = @"/files/" + fileName;
            return result;
        }

        public IActionResult IndexCard(string category)
        {
            if(category == null || category.Trim() == "")
            {
                category = "%%";
            }

            _logger.LogInformation("게시판 리스트 Card 로딩");
            _repository.Log("IndexCard", HttpContext.Connection.RemoteIpAddress.ToString());

            // 게시판 리스트 정보 가져오기
            List<Note> notes = new List<Note>();
            TotalRecordCount = _repository.GetCountAll();
            notes = _repository.GetCardAll(category);

            // 카테고리 Count Get
            CategoryList categoryList = _repository.GetCategoryCnt();
            if (category != "%%") ViewBag.Category = category;
            ViewBag.Project = categoryList.Project;
            ViewBag.Database = categoryList.Datbase;
            ViewBag.Anything = categoryList.Anything;
            ViewBag.Algorithm = categoryList.Algorithm;

            // 주요 정보를 뷰 페이지로 전송
            ViewBag.TotalRecord = TotalRecordCount;
            ViewBag.SearchMode = SearchMode;
            ViewBag.SearchField = SearchField;
            ViewBag.SearchQuery = SearchQuery;
            ViewBag.Cnt = 0;

            return View(notes);
        }

        public IActionResult Index(string category , int noteId)
       {
            if (category == null) category = "%%";
            _logger.LogInformation("게시판 리스트 페이지 로딩");

            // For Log...
            if(noteId != 0)
                _repository.Log("IndexGrid:" + noteId, HttpContext.Connection.RemoteIpAddress.ToString());
            else
                _repository.Log("IndexGrid", HttpContext.Connection.RemoteIpAddress.ToString());


            // [0] 검색창 로직 검색 모드 결정: ?SearchField=Name&SearchQuery=닷넷코리아 */
            SearchMode = (
                !string.IsNullOrEmpty(Request.Query["SearchField"]) &&
                !string.IsNullOrEmpty(Request.Query["SearchQuery"])
            );
            if (SearchMode)
            {
                SearchField = Request.Query["SearchField"].ToString();
                SearchQuery = Request.Query["SearchQuery"].ToString();
            }

            //[1] 쿼리스트링에 따른 페이지 보여주기
            if (!string.IsNullOrEmpty(Request.Query["Page"].ToString()))
            {
                // Page는 보여지는 쪽은 1, 2, 3, ... 코드단에서는 0, 1, 2, ...
                PageIndex = Convert.ToInt32(Request.Query["Page"]) - 1;
            }

            //[2] 쿠키를 사용한 리스트 페이지 번호 유지 적용(Optional): 
            if (!string.IsNullOrEmpty(Request.Cookies["DotNetNotePageNum"]))
            {
                if (!String.IsNullOrEmpty(Request.Cookies["DotNetNotePageNum"]))
                    PageIndex = Convert.ToInt32(Request.Cookies["DotNetNotePageNum"]);
                else
                    PageIndex = 0;
            }


            //[3] 게시판 리스트 정보 가져오기 
            List<Note> notes = new List<Note>();
            NoteCommentViewModel vm = new NoteCommentViewModel();
            bool isSelected = false;
            int LatestId = 0;

            // 게시판 글 Get 
            if (!SearchMode)
            {
                TotalRecordCount = _repository.GetCountAll();
                notes = _repository.GetAll(PageIndex, category);
            }
            else
            {
                TotalRecordCount = _repository.GetCountBySearch(SearchField, SearchQuery);
                notes = _repository.GetSearchAll(PageIndex, SearchField, SearchQuery);
            }


            //[4] 사용자 선택 글 or 최신글 Get 
            foreach (Note list in notes)
            {
                if (LatestId == 0) LatestId = list.Id;

                if(list.Id == noteId)
                {
                    ViewBag.Title = list.Title;
                    list.isMain = true;
                    isSelected = true;
                    break;
                }
            }

            //[5] 댓글 창 Get
            if (isSelected)
            {
                vm.NoteCommentList = _commentRepository.GetNoteComments(noteId);
                vm.BoardId = noteId;
                _repository.GetNoteById(noteId, false);
            }
            else
            {
                if (notes.Count != 0) notes[0].isMain = true;
                vm.NoteCommentList = _commentRepository.GetNoteComments(LatestId);
                vm.BoardId = LatestId;
            }
            ViewBag.CommentListAndId = vm;

            //[6] 카테고리 Count Get
            CategoryList categoryList = _repository.GetCategoryCnt();


            //[7] 주요 정보를 뷰 페이지로 전송
            ViewBag.TotalRecord = TotalRecordCount;
            ViewBag.SearchMode = SearchMode;
            ViewBag.SearchField = SearchField;
            ViewBag.SearchQuery = SearchQuery;
            ViewBag.PageIndex = PageIndex;

            if (category != "%%")  ViewBag.Category = category;
            ViewBag.Project = categoryList.Project;
            ViewBag.Database = categoryList.Datbase;
            ViewBag.Anything = categoryList.Anything;
            ViewBag.Algorithm = categoryList.Algorithm;

            //[8] 페이저 컨트롤 적용
            ViewBag.PageModel = new PagerBase
            {
                Url = "DotNetNote/Index",
                RecordCount = TotalRecordCount,
                PageSize = 5,
                PageNumber = PageIndex + 1,
                Category= category,

                SearchMode = SearchMode,
                SearchField = SearchField,
                SearchQuery = SearchQuery
            };

            return View(notes);
        }


        /// <summary>
        /// 게시판 글쓰기 폼 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.FormType = BoardWriteFormType.Write;
            ViewBag.TitleDescription = "글 쓰기 - 다음 필드들을 채워주세요.";
            ViewBag.SaveButtonText = "저장";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Note model, ICollection<IFormFile> files)
        {
            string fileName = string.Empty;
            int fileSize = 0;
            string Email = User.FindFirst("Email").Value;
            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    fileName = CommonLibrary.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.ToString()));

                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            Note note = new Note();
            note.Email = CommonLibrary.HtmlUtility.Encode(Email);
            note.HomePage = model.HomePage;
            note.Title = CommonLibrary.HtmlUtility.Encode(model.Title);

            if (model.Title == null || model.Content == "")
            {
                ModelState.AddModelError("", "내용을 기입해주세요");
                return View(model);
            }

            note.Content = model.Content;
            note.FileName = fileName;
            note.FileSize = fileSize;
            note.Password = new CommonLibrary.Security().EncryptPassword(model.Password);
            note.PostIp = HttpContext.Connection.RemoteIpAddress.ToString();
            note.Encoding = model.Encoding;
            note.Category = model.Category;

            _repository.Add(note); // 데이터 저장

            TempData["Message"] = "데이터가 저장되었습니다";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 게시판 파일 강제 다운로드 기능(/BoardDown/:Id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult BoardDown(int id)
        {
            string fileName = "";

            fileName = _repository.GetFileNameById(id);

            if (fileName == null)
            {
                return null;
            }
            else
            {
                _repository.UpdateDownCountById(id);

                byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(
                    _environment.WebRootPath, "files") + "//" + fileName);

                return File(fileBytes, "application/octet-stream", fileName);
            }
        }

        /// <summary>
        /// 게시판의 상세보기 페이지 (Detatils, BoardView)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            // 로깅
            _logger.LogInformation("Detail 페이지 로딩");
            _repository.Log("Detail : " + id, HttpContext.Connection.RemoteIpAddress.ToString());

            var note = _repository.GetNoteById(id, true);

            note.PostDates = note.PostDate.ToString();

            if (note == null)
            {
                return RedirectToAction("DeleteCompleted");
            }

            ContentEncodingType encoding = (ContentEncodingType)Enum.Parse(
                typeof(ContentEncodingType), note.Encoding);

            string encodedContent = "";
            encoding = ContentEncodingType.Mixed;
            switch (encoding)
            {
                case ContentEncodingType.Text:
                    encodedContent =
                        CommonLibrary.HtmlUtility.Encode(note.Content);
                    break;
                case ContentEncodingType.Html:
                    encodedContent = note.Content;
                    break;
                case ContentEncodingType.Mixed:
                    encodedContent = note.Content.Replace("\r\n", "<br />");
                    break;
                default:
                    encodedContent = note.Content;
                    break;
            }
            ViewBag.Content = encodedContent;
            ViewBag.Title = note.Title;

            if (note.FileName.Length > 1)
            {
                if (note.FileName.Length > 10)
                {
                    note.FileName = note.FileName.Substring(0, 10);
                }
                ViewBag.FileName = String.Format(
                    "<a href='/DotNetNote/BoardDown?Id={0}'>"
                    + "{1}{2} / 전송수 {3}</a>", note.Id
                    , "<img src=\"/images/ext/ext_zip.gif\" border=\"0\">"
                    , note.FileName, note.DownCount);

                if (CommonLibrary.BoardLibrary.IsPhoto(note.FileName))
                {
                    ViewBag.ImageDown =
                        $"<img src\'/DotNetNote/ImageDown/{note.Id}\'><br />";
                }
            }
            else
            {
                ViewBag.FileName = "{업로드된 파일이 없습니다}";
            }

            NoteCommentViewModel vm = new NoteCommentViewModel();
            vm.NoteCommentList = _commentRepository.GetNoteComments(note.Id);
            vm.BoardId = note.Id;
            ViewBag.CommentListAndId = vm;

            return View(note);
        }


        // 게시판 삭제 폼
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Id = id;
            return View();
        }


        //게시판 삭제 처리
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, string password)
        {
            // 로깅
            _logger.LogInformation("Delete 페이지 로딩");
            _repository.Log("Delete : " + id, HttpContext.Connection.RemoteIpAddress.ToString());

            password = new CommonLibrary.Security().EncryptPassword(password);
            if (_repository.DeleteNote(id, password) > 0)
            {
                TempData["Message"] = "데이터가 삭제되었습니다";

                return RedirectToAction("DeleteCompleted");
            }
            else
            {
                ViewBag.Message = "삭제되지 않았습니다. 비번을 확인하세요";
            }

            ViewBag.Id = id;

            return View();
        }


        // 게시판 삭제 완료 후 추가적인 처리할 때 페이지
        public IActionResult DeleteCompleted()
        {
            // 로깅
            _logger.LogInformation("Delete 페이지 로딩");
            _repository.Log("Deleted", HttpContext.Connection.RemoteIpAddress.ToString());
            return View();
        }

        // 게시판 수정 폼
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.FormType = BoardWriteFormType.Modify;
            ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
            ViewBag.SaveButtonText = "수정";

            var note = _repository.GetNoteById(id, true);

            if (note.FileName.Length > 1)
            {
                ViewBag.FileName = note.FileName;
                ViewBag.FileSize = note.FileSize;
                ViewBag.FileNamePrevious = $"기존에 업로드된 파일명: {note.FileName}";
            }
            else
            {
                ViewBag.FileName = "";
                ViewBag.FileSize = 0;
            }
            return View(note);
        }


        // 게시판 수정 처리 + 파일 업로드 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Note model, ICollection<IFormFile> files,
            int id, string previousFileName = "", int previousFileSize = 0)
        {
            ViewBag.FormType = BoardWriteFormType.Modify;
            ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
            ViewBag.SaveButtonText = "수정";

            string fileName = "";
            int fileSize = 0;

            if (previousFileName != null)
            {
                fileName = previousFileName;
                fileSize = previousFileSize;
            }

            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    fileName = CommonLibrary.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.ToString()));

                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            Note note = new Note();

            note.Id = id;
            note.Email = User.FindFirst("Email").Value;
            note.HomePage = model.HomePage;
            note.Title = CommonLibrary.HtmlUtility.Encode(model.Title);

            if (model.Title == null || model.Content == "")
            {
                ModelState.AddModelError("", "내용을 기입해주세요");
                return View(model);
            }

            note.Content = model.Content;
            note.FileName = fileName;
            note.FileSize = fileSize;
            note.Password = new CommonLibrary.Security().EncryptPassword(model.Password);
            note.ModifyIp = HttpContext.Connection.RemoteIpAddress.ToString();
            note.Encoding = model.Encoding;

            int r = _repository.UpdateNote(note);

            if (r > 0)
            {
                TempData["Message"] = "수정 되었습니다.";
                return RedirectToAction("Details", new { Id = id });
            }
            else
            {
                ViewBag.ErrorMessage = "업데이트가 되지 않았습니다. 암호를 확인하세요";
                return View(note);
            }
        }


        [HttpGet]
        public IActionResult Reply(int id)
        {
            ViewBag.FormType = BoardWriteFormType.Reply;
            ViewBag.TitleDescription = "글 답변 - 다음 필드들을 채워주세요";
            ViewBag.SaveButtonText = "답변";

            var note = _repository.GetNoteById(id, false);

            var newNote = new Note();

            newNote.Title = $"Re: {note.Title}";
            newNote.Content =
                $"\r\n <----------'{note.PostDate}'\n----------\n>"
                + $"{note.Content.Replace("\n", "\n>")}\n----------";

            return View(newNote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(
            Note model, ICollection<IFormFile> files, int id)
        {
            string fileName = string.Empty;
            int fileSize = 0;

            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    fileName = CommonLibrary.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.ToString()));

                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            Note note = new Note();

            note.Id = note.ParentNum = Convert.ToInt32(id);
            note.Email = User.FindFirst("Email").Value;
            note.HomePage = model.HomePage;
            note.Title = CommonLibrary.HtmlUtility.Encode(model.Title);

            if (model.Title == null || model.Content == "")
            {
                ModelState.AddModelError("", "내용을 기입해주세요");
                return View(model);
            }

            note.Content = model.Content;
            note.FileName = fileName;
            note.FileSize = fileSize;
            note.Password = new CommonLibrary.Security().EncryptPassword(model.Password);
            note.PostIp = HttpContext.Connection.RemoteIpAddress.ToString();
            note.Encoding = model.Encoding;

            _repository.ReplyNote(note);

            TempData["Message"] = "데이터가 저장되었습니다";
            return RedirectToAction("Index");
        }


        /// <summary>
        /// ImageDown: 완성형 게시판의 이미지 전용 다운 페이지
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ImageDown(int id)
        {
            // 로깅
            _logger.LogInformation("ImageDown 로딩");
            _repository.Log("ImageDown", HttpContext.Connection.RemoteIpAddress.ToString());
            string fileName = "";

            fileName = _repository.GetFileNameById(id);

            if (fileName == null)
            {
                return null;
            }
            else
            {
                string strFileName = fileName;
                string strFileExt = Path.GetExtension(strFileName);
                string strContentType = "";

                if (strFileExt == ".gif" || strFileExt == ".jpg" ||
                    strFileExt == ".jpeg" || strFileExt == ".png")
                {
                    switch (strFileExt)
                    {
                        case ".gif":
                            strContentType = "image/gif"; break;
                        case ".jpg":
                            strContentType = "image/jpeg"; break;
                        case ".jpeg":
                            strContentType = "image/jpeg"; break;
                        case ".png":
                            strContentType = "image/png"; break;
                    }
                }
                if (System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName))
                {
                    // 다운로드 카운트 증가 메서드 호출
                    _repository.UpdateDownCount(fileName);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(
                        _environment.WebRootPath, "files") + "\\" + fileName);

                    // 이미지 파일 다운로드 
                    return File(fileBytes, strContentType, fileName);
                }

                return Content("http://placehold.it/250x150?text=NoImage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentAdd(
            int BoardId, string txtName, string txtPassword, string txtOpinion)
        {
            NoteComment comment = new NoteComment();
            comment.BoardId = BoardId;
            comment.Name = txtName;
            comment.Password = new CommonLibrary.Security().EncryptPassword(txtPassword);
            comment.Opinion = txtOpinion;


            _commentRepository.AddNoteComment(comment);

            return RedirectToAction("Details", new { id = BoardId });
        }

        [HttpGet]
        public IActionResult CommentDelete(string boardId, string id)
        {
            ViewBag.BoardId = boardId;
            ViewBag.Id = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentDelete(string boardId, string id, string txtPassword)
        {
            txtPassword = new CommonLibrary.Security().EncryptPassword(txtPassword);
            if (_commentRepository.GetCountBy(Convert.ToInt32(boardId), Convert.ToInt32(id), txtPassword) > 0)
            {
                _commentRepository.DeleteNoteComment(Convert.ToInt32(boardId), Convert.ToInt32(id), txtPassword);

                return RedirectToAction("Details", new { id = boardId });
            }

            ViewBag.BoardId = boardId;
            ViewBag.Id = id;
            ViewBag.Password = txtPassword;
            ViewBag.ErrorMessage = "암호가 틀립니다. 다시 입력해주세요";
            return View();
        }

        [HttpGet]
        [Authorize("Admin")]
        public IActionResult Pinned(int id)
        {
            _repository.Pinned(id);
            return RedirectToAction("index");
        }

        // 최근 글 리스트 Web Api 테스트 페이지 
        public IActionResult NoteServiceDemo()
        {
            return View();
        }

        public IActionResult NoteCommentServiceDemo()
        {
            return View();
        }
    }
}
