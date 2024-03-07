using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Class;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using FiltrDinamico.Core.Interpreters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CursoIgrejaApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Conexao com base de dados
            services.AddDbContext<DataContext>(options =>
                     options.UseMySql(Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());

            CarregaInjecaoDependencia(services);

            //Servico da autenticacao Jwt
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //Ignora o circle recursivo
             services.AddControllersWithViews().AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );

            //Servico da documentacao do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DaumTok API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {new OpenApiSecurityScheme{ Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    }, new List<string>() }
                 });
            });

            //Servico responsavel por deixar por padrao as chamadas da controller protegidas
            services.AddControllers(
                options =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
            );

            //Servico AutoMapper
            services.AddAutoMapper(typeof(Startup));

            services.AddCors();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase("/api");

            app.Use((context, next) =>
            {
                context.Request.PathBase = "/api";
                return next();
            });

            //app.UseHttpsRedirection();

            app.UseSwagger();


            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API V1");
            });


            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseStaticFiles();

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"arq_anexos")),
            //    RequestPath = new PathString("/arq_anexos")
            //});

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"arq_videos")),
            //    RequestPath = new PathString("/arq_videos")
            //});
        }


        private static void CarregaInjecaoDependencia(IServiceCollection services)
        {
            services.AddScoped<IFiltroDinamico, FiltroDinamico>();
            services.AddScoped<IFilterInterpreterFactory, FilterInterpreterFactory>();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IRepositoryBaseView<>), typeof(RepositoryViewBase<>));
            services.AddScoped<IUsuariosRepository, UsuariosRepository>();
            services.AddScoped<ICongregacaoRepository, CongregacaoRepository>();
            services.AddScoped<ICursoRepository, CursoRepository>();
            services.AddScoped<IProcessoInscricaoRepository, ProcessoInscricaoRepository>();
            services.AddScoped<IInscricaoUsuarioRepository, InscricaoUsuarioRepository>();
            services.AddScoped<IMeioPagamentoRepository, MeioPagamentoRepository>();
            services.AddScoped<IParametroSistemaRepository, ParametroSistemaRepository>();
            services.AddScoped<ITransacaoInscricaoRepository, TransacaoInscricaoRepository>();
            services.AddScoped<ILogNotificacoesRepository, LogNotificacoesRepository>();
            services.AddScoped<ILogUsuarioRepository, LogUsuarioRepository>();
            services.AddScoped<IModuloRepository, ModuloRepository>();
            services.AddScoped<IConteudoRepository, ConteudoRepository>();
            services.AddScoped<IConteudoUsuarioRepository, ConteudoUsuarioRepository>();
            services.AddScoped<IProvaUsuarioRepository, ProvaUsuarioRepository>();
            services.AddScoped<IUsuarioSistemaRepository, UsuarioSistemaRepository>();
            services.AddScoped<IProfessorRepository, ProfessorRepository>();
            services.AddScoped<IProvaRepository, ProvaRepository>();
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<IVwContagemInscricaoCongregacaoRepository, VwContagemInscricaoCongregacaoRepository>();
            services.AddScoped<IVwContagemInscricaoCursoRepository, VwContagemInscricaoCursoRepository>();
            services.AddScoped<IInscricaoLiberarCursoRepository,InscricaoLiberarCursoRepository>();
            services.AddScoped<IGeolocalizacaoUsuarioRepository, GeolocalizacaoUsuarioRepository>();
            services.AddScoped<IPresencaUsuarioRepository, PresencaUsuarioRepository>();
            services.AddScoped<IVwRelatorioInscricoes, VwRelatorioInscricoesRepository>();
            services.AddScoped<IRelatorioGeraisRepository, RelatorioGeraisRepository>();

        }
    }
}
