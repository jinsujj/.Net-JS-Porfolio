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

        /// [5] DB�� json ���� ���� �б� ���� _Config �Ű����� ���� 
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /// [4] MVC ���� ���
            services.AddControllersWithViews();

            /// [9-1] // DotNetNoteSettings.json ������ �����͸� POCO Ŭ������ ����
            services.Configure<DotNetNoteSetting>(_config.GetSection("DotNetNoteSetting"));

            /// [9-2] DotNetNoteSettings.json �о� ���� ��������
            services.AddAuthorization(options =>
            {
                // User Role �� ������ Users Policy �ο�
                options.AddPolicy("Users", policy => policy.RequireRole("Users"));

                // User Role �� �ְ� UserId �� DotNetNoteSetting:SiteAdmin �� ������ ��(���� ��� "Admin")�̸� "Admin" �ο�
                //options.AddPolicy("Admin", policy => policy.RequireRole("UsersInfo").RequireClaim("Email", _config.GetSection("DotNetNoteSetting").GetSection("SiteAdmin").Value));
                options.AddPolicy("Admin", policy => policy.RequireRole("UsersInfo").RequireClaim("Email", "wlstncjs1234@naver.com"));
            });

            /// [7] ��� ���� �⺻
            services.AddAuthentication("Cookies").AddCookie(options =>
            {
                options.LoginPath = "/Account/Login/";
                options.AccessDeniedPath = "/Account/Forbidden/";
            });

            /// [6] (Dapper) Query Repository ���� 
            /*
             * Transient  �� Lifetime services  �� �Ź� �������̽��� ��û�� ������ ���ο� ��ü�� �����մϴ�.�̴� ������(stateless)  ���񽺿� ���� �����մϴ�.
             * Scoped �� Lifetime services ���, �� HTTP ��û �� �ϳ��� �ν��Ͻ��� �����ϸ�, ������ �ּ��� ��ȭ�鳻�� ������ �̸� ����� ��� ������ �ν��Ͻ��� �����մϴ�.
             * Singleton Lifetime services �� �� �ѹ� ó������ �ν��Ͻ��� �����ϰ�, ��� ȣ�⿡�� ������ ������Ʈ�� �����մϴ�. ���� ������� ��ü ��� ȣ���ϴ� ������ �����ϴ� ȿ���� ����ϴ�. ���� ������ �湮�� ���� ������ �� ����ϸ� �ǰڽ��ϴ�.
             */
            services.AddSingleton<IConfiguration>(_config);  //appsettings.json ������ �����ͺ��̽� ���ڿ� ����ϵ��� ���� 
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

            /// [1]wwwroot ���� ���� ������ ��� ���� 
            app.UseStaticFiles();

            /// [2] Routing ��� 
            app.UseRouting();

            /// [8] ���� ���� Ȱ��ȭ
            app.UseAuthentication();
            app.UseAuthorization();

            /// ������ ���� 
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            /// [3] ���� ���� ����
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
