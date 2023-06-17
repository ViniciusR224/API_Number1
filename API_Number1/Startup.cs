using API_Number1.AppDbContext;


using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IRepository_Base;
using API_Number1.Interfaces.IUser_Repository;


using API_Number1.Repositories.Repository_Base;
using API_Number1.Repositories.User_Repository;

using API_Number1.Services.FactoryBase;
using API_Number1.Services.Jwt_Service;
using API_Number1.Services.PasswordHasher_Service;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API_Number1.Middlewares;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using API_Number1.GlobalError;
using System.Diagnostics;

namespace API_Number1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection Services)
        {


            Services.AddControllers().AddNewtonsoftJson();

            // Adicionar suporte ao Swagger para documentação da API
            Services.AddSwaggerGen();

            // Configurar o banco de dados
            Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MySQL")));

            // Registrar os serviços e repositórios
            Services.AddScoped<IPasswordHasher, PasswordHasherService>();
            Services.AddScoped(typeof(IRepostoryBase<>), typeof(RepositoryBase<>));
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<IJwtService, JwtGeneratorSevice>();
            Services.AddScoped<IServiceBase, BaseService>();
            Services.AddTransient<ExceptionMiddleware>();
            // Configurações adicionais dos serviços podem ser adicionadas aqui


            // Configuração de outros serviços como autenticação, autorização, etc.
            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]))
                };
            });
            Services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware> ();

            ////Útil para casos de handling global de exceptions, não somente em controllers por exemplo
            ////No caso esse é responsável por capturar essas exceções não tratadas globalmente e fornecer uma resposta consistente para elas.
            ////Exceções podem ocorrer em diferentes partes da aplicação, como middleware, pipelines de solicitação, etc.
            ////ExceptionHandler é um middleware no pipeline de solicitação do ASP.NET Core. 
            ////Eu poderia fazer um middleware também, um que implementa IMiddleware, para não ter que deixar tudo aqui na startup, que faça o mesmo
            
            //app.UseExceptionHandler(
            //   options =>
            //   {
            //       options.Run(async context =>
            //       {
            //           context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //           context.Response.ContentType = "application/json";
            //           //Fornece acesso às informações sobre uma exceção que foi capturada pelo UseExceptionHandler.
            //           //Essa interface é usada para recuperar detalhes da exceção, como o tipo de exceção, a mensagem
            //           //de erro e outras informações relevantes.
            //           var objectException = context.Features.Get<IExceptionHandlerFeature>();
            //           //Nesse caso de usar o handler, tenho que "puxar"
            //           //a exception em si, até porque como o nome diz ele só possui o context, só faz o handling dela
            //           var message = new
            //           {
            //               Error = objectException.Error.Message,
            //               ErrorCode ="ERR_",
            //               Timestamp = DateTime.UtcNow,
            //               InnerException = objectException.Error.InnerException?.Message,
            //               AdditionalInfo = new
            //               {
            //                   Context = "Mensagem Padrão de Exception"                              
            //               }
            //           };
                       
            //           await context.Response.WriteAsJsonAsync(message);
            //       });
            //   });



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }


    }
}
