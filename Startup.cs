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

            /// [6] (Dapper) Query Repository ���� 
            /*
             * Transient  �� Lifetime services  �� �Ź� �������̽��� ��û�� ������ ���ο� ��ü�� �����մϴ�.�̴� ������(stateless)  ���񽺿� ���� �����մϴ�.
             * Scoped �� Lifetime services ���, �� HTTP ��û �� �ϳ��� �ν��Ͻ��� �����ϸ�, ������ �ּ��� ��ȭ�鳻�� ������ �̸� ����� ��� ������ �ν��Ͻ��� �����մϴ�.
             * Singleton Lifetime services �� �� �ѹ� ó������ �ν��Ͻ��� �����ϰ�, ��� ȣ�⿡�� ������ ������Ʈ�� �����մϴ�. ���� ������� ��ü ��� ȣ���ϴ� ������ �����ϴ� ȿ���� ����ϴ�. ���� ������ �湮�� ���� ������ �� ����ϸ� �ǰڽ��ϴ�.
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

            /// [1]wwwroot ���� ���� ������ ��� ���� 
            app.UseStaticFiles();

            /// [2] Routing ��� 
            app.UseRouting();

            /// https ��� [ �������� ������ ��� ��û�� ���� SSL�������� �ε��ϵ��� ���� ]
            //app.UseHttpsRedirection();

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
