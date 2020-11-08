using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Data;
using Microsoft.EntityFrameworkCore.Design;
using MyApp.Data.Repositorys;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;

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

            /// [6] (Dapper) Query Repository 연동 
            /*
             * Transient  의 Lifetime services  는 매번 인터페이스가 요청될 때마다 새로운 객체를 생성합니다.이는 비유지(stateless)  서비스에 가장 적합합니다.
             * Scoped 의 Lifetime services 경우, 각 HTTP 요청 당 하나의 인스턴스를 생성하며, 동일한 주소의 웹화면내서 여러번 이를 사용할 경우 동일한 인스턴스를 재사용합니다.
             * Singleton Lifetime services 는 딱 한번 처음으로 인스턴스를 생성하고, 모든 호출에서 동일한 오브젝트를 재사용합니다. 따라서 결과값을 전체 모든 호출하는 사람들과 공유하는 효과를 얻습니다. 보통 누적된 방문자 수를 보여줄 때 사용하면 되겠습니다.
             */
            services.AddSingleton<IConfiguration>(_config);
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
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

            /// https 사용 [ 웹서버가 들어오는 모든 요청에 대해 SSL인증서를 로드하도록 지시 ]
            //app.UseHttpsRedirection();

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
