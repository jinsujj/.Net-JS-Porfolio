using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Data.Repositorys;
using Microsoft.AspNetCore.HttpOverrides;
using MyApp.Data.Repositorys.Login;
using MyApp.Data.Repositorys.DotNetNote;
using System.Security.Claims;
using MyApp.Settings;
using MyApp.Data.Repositorys.DashBoard;

namespace MyApp
{
    public class Startup
    {
        public IConfiguration _config { get; }

        /// [5] DB등 json 설정 파일 읽기 위한 _Config 매개변수 선언 
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /// [4] MVC 서비스 등록
            services.AddControllersWithViews();

            /// [9-1] // DotNetNoteSettings.json 파일의 데이터를 POCO 클래스에 주입
            services.Configure<DotNetNoteSetting>(_config.GetSection("DotNetNoteSetting"));

            /// [9-2] DotNetNoteSettings.json 읽어 권한 가져오기
            services.AddAuthorization(options =>
            {
                // User Role 이 있으면 Users Policy 부여
                options.AddPolicy("Users", policy => policy.RequireRole("Users"));

                // User Role 이 있고 UserId 가 DotNetNoteSetting:SiteAdmin 에 지정된 값(예를 들어 "Admin")이면 "Admin" 부여
                //options.AddPolicy("Admin", policy => policy.RequireRole("UsersInfo").RequireClaim("Email", _config.GetSection("DotNetNoteSetting").GetSection("SiteAdmin").Value));
                options.AddPolicy("Admin", policy => policy.RequireRole("UsersInfo").RequireClaim("Email", "wlstncjs1234@naver.com"));
            });

            /// [7] 쿠기 인증 기본
            services.AddAuthentication("Cookies").AddCookie(options =>
            {
                options.LoginPath = "/Account/Login/";
                options.AccessDeniedPath = "/Account/Forbidden/";
            });

            /// [6] (Dapper) Query Repository 연동 
            /*
             * Transient  의 Lifetime services  는 매번 인터페이스가 요청될 때마다 새로운 객체를 생성합니다.이는 비유지(stateless)  서비스에 가장 적합합니다.
             * Scoped 의 Lifetime services 경우, 각 HTTP 요청 당 하나의 인스턴스를 생성하며, 동일한 주소의 웹화면내서 여러번 이를 사용할 경우 동일한 인스턴스를 재사용합니다.
             * Singleton Lifetime services 는 딱 한번 처음으로 인스턴스를 생성하고, 모든 호출에서 동일한 오브젝트를 재사용합니다. 따라서 결과값을 전체 모든 호출하는 사람들과 공유하는 효과를 얻습니다. 보통 누적된 방문자 수를 보여줄 때 사용하면 되겠습니다.
             */
            services.AddSingleton<IConfiguration>(_config);  //appsettings.json 파일의 데이터베이스 문자열 사용하도록 설정 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILoginFailedRepository, LoginFailedRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<IDashBoardRepository, DashBoardRepository>();
            services.AddSingleton<INoteCommentRepository>(
                new NoteCommentRepository(
                    _config["ConnectionStrings:DefaultConnection"]));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /// [1]wwwroot 정적 파일 보관소 사용 여부 
            app.UseStaticFiles();

            /// [2] Routing 사용 
            app.UseRouting();

            /// [8] 인증 서비스 활성화
            app.UseAuthentication();
            app.UseAuthorization();

            /// 리눅스 배포 
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            /// [3] 시작 지점 설정
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
