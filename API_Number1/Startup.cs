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
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using API_Number1.Filters;
using API_Number1.DependencyInjectionConfig;

namespace API_Number1
{
    //Tenho que adicionar o caching e refatorar algumas coisas dos exceptions middleware, também adicionar os formatadore e versionamento, log
    //Estudar e configurar tudo, seja para as exception,caching e tudo mais
    
    //Validações - FluentValidation e talvez um MediatR
    //Caching - Redis configurado da maneira certa
    //Loggings - Serilog
    //Talvez mudar a questão dos middlewares de exceptions e o retorno de problem details
    //Ver se está configurado corretamente a questão de formatadores se eu não mudar o middleware de exception
    //Talvez refatorar algumas coisas utilizando a técnica do vídeo
    //Configurar tudo corretamente para o env para produção 
    //Ver sobre versionamento
    //Ver também sobre documentação da api

    //Talvez colocar tudo em clean architecture e cqrs seguindo o vídeo
    //Refatorar tudo, desde as validações até o processos de cada coisa e a questão da arquitetura também 
    //Refatorar para tirar os servicços e repositories dos controllers

    //Só use o secrets.json se for para o env dev
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection Services)
        {

            Services.AddControllerAndSweggerSupport()
                .AddDbContextConfiguration(Configuration)
                //.AddCorsSupport() As configurações já estão prontas, use caso utilize uma outra origin/front-end em outro localhost
                //Como isso aqui é uma api para estudos, vou deixar desativado
                //Mas caso queira testar o servidor esta na pasta ServerLocalTest, é só dar um node  server.js 
                .AddServicesAndCollections()
                .AddAuthorizathionAndAuthentication(Configuration);


                ////Estou criando uma policy aqui porque fazer no AddAuthorization seria mais para uma situação global, não somente nos controllers
                ////Fica aqui configurado que todos os usuarios devem estar authenticados para conseguir a authorização de usar os métodos
                //Services.AddControllers(options =>
                //{
                //    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole("User", "Administrator").Build();
                //    //Exceptions nivel controller
                //    
                //    options.Filters.Add(new AuthorizeFilter(policy));
                //    options.Filters.Add<ExceptionFilterController>();
                //
                //}).AddNewtonsoftJson();
                //
                //// Adicionar suporte ao Swagger para documentação da API
                //Services.AddSwaggerGen();
                
                //// Configurar o banco de dados
                //Services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseMySQL(Configuration.GetConnectionString("MySQL")));
                //
                // // Registrar os serviços e repositórios
                // Services.AddScoped<IPasswordHasher, PasswordHasherService>();
                // Services.AddScoped(typeof(IRepostoryBase<>), typeof(RepositoryBase<>));
                // Services.AddScoped<IUserRepository, UserRepository>();
                // Services.AddScoped<IJwtService, JwtGeneratorSevice>();
                // Services.AddScoped<IServiceBase, BaseService>();
                // Services.AddTransient<ExceptionMiddleware>();
                // // Configurações adicionais dos serviços podem ser adicionadas aqui
                //
                //
                // // Configuração de outros serviços como autenticação, autorização, etc.
                // Services.AddAuthentication(options =>
                // {
                //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //
                // })
                //.AddJwtBearer(options =>
                //{
                //    options.TokenValidationParameters = new TokenValidationParameters
                //    {
                //        ValidateIssuer = true,
                //        ValidateAudience = true,
                //        ValidateLifetime = true,
                //        ValidateIssuerSigningKey = true,
                //        ValidIssuer = Configuration["Jwt:Issuer"],
                //        ValidAudience = Configuration["Jwt:Audience"],
                //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]))
                //    };
                //});
                // Services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Global Exception Middleware
            //app.UseMiddleware<ExceptionMiddleware> ();

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
            //           //Mas perceba que de se certa forma é o mais limitado sobre a questão de informações que podem ser obtidas 
            //           //Perdendo somente para a abordagem que só possui o catch da exception em si, que não tem o context nele.
            //           //Ele já possui umas informações já prontas que tendem a ser uteis da exception, como path e coisas do tipo
            //           //Dai logo não preciso acessar o Httpcontext para obter por exemplo
            //           var message = new
            //           {
            //               Error = objectException.Error.Message,
            //               ErrorCode = "ERR_",
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

            //app.UseCors("Prod");                   

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



        }


    }
}
