using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Filters;
using API_Number1.Interfaces.IADM_Service;
using API_Number1.Interfaces.IAuthenticationProcess;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IPatchProcess;
using API_Number1.Interfaces.IRepository_Base;
using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.ISigUpProcess;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Interfaces.IUserService;
using API_Number1.Interfaces.ValidationInterfaces;
using API_Number1.Middlewares;
using API_Number1.Models;
using API_Number1.Repositories.Repository_Base;
using API_Number1.Repositories.User_Repository;
using API_Number1.Repositories.Validation_Repository;
using API_Number1.Services.Adm_Service;
using API_Number1.Services.AuthenticationProcessService;
using API_Number1.Services.FactoryBase;
using API_Number1.Services.Jwt_Service;
using API_Number1.Services.PasswordHasher_Service;
using API_Number1.Services.Patch_Process;
using API_Number1.Services.SignUpProcessService;
using API_Number1.Services.User_Service;
using API_Number1.Validations;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace API_Number1.DependencyInjectionConfig
{
    public static class DependencyInjection
    {
        //Teoricamente se fizessemos com a clean, isso seria separado em AddAplication e AddPersistance por exemplo, em cada camada
        //conteria os métodos necessários/da camada em si. 
        
        //Classe para configuração dos Services na startup - Funcinará como uma extension - Forma de estruturar DI
        public static IServiceCollection AddControllerAndSweggerSupport(this IServiceCollection Services)
        {

            Services.AddControllers(options =>
            {
                ConfigureFiltersAdditions(options);


            })
            .AddNewtonsoftJson()
            .ConfigureFormatters();


            // Adicionar suporte ao Swagger para documentação da API
            Services.AddSwaggerGen();


            return Services;
        }
        //Configurações de formatadores para diferentes tipos em caso de retorno de um ObjectResult em tipos diferentes. 
        public static void ConfigureFormatters(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            })
            .AddXmlSerializerFormatters();
           
        }
        //Configurações de filtro.
        public static void ConfigureFiltersAdditions(MvcOptions  options)
        {
            ////Estou criando uma policy aqui porque fazer no AddAuthorization seria mais para uma situação global, não somente nos controllers
            ////Fica aqui configurado que todos os usuarios devem estar authenticados para conseguir a authorização de usar os métodos
            //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole("User", "Administrator").Build();
            //options.Filters.Add(new AuthorizeFilter(policy));
            
            //Exceptions nivel controller
            //options.Filters.Add<ExceptionFilterController>();
        }





        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MySQL")));

            return Services;

        }

        public static IServiceCollection AddServicesAndCollections(this IServiceCollection Services)
        {
            //Registrar os serviços e repositórios                       
            Services.AddScoped(typeof(IRepostoryBase<>), typeof(RepositoryBase<>));
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped(typeof(IValidationRepository<>), typeof(ValidationRepository<>));


            Services.AddScoped<IValidator<SignUpRequest>, SignUpValidation>();
            Services.AddScoped<IUserPatchValidation, UserPatchValidation2>();
            

            Services.AddScoped<IPasswordHasher, PasswordHasherService>();
            Services.AddScoped<IServiceBase, BaseService>();
            Services.AddScoped<IJwtService, JwtGeneratorSevice>();                        
            Services.AddScoped<IAuthentication_Process, AuthenticationProcess>();
            Services.AddScoped<IPatch_Process, PatchProcess>();
            Services.AddScoped<IUser_Service, UserService>();
            Services.AddScoped<IAdm_Service, AdmService>();



            Services.AddScoped<ISignUpProcess, SignUpProcess>();
            Services.AddScoped<IAuthentication_Process, AuthenticationProcess>();

            //Services.AddTransient<ExceptionMiddleware>();

            //Middleware de Exception.
            Services.AddProblemDetails(options =>
            {
                //Se você quer explorar os opções vá nas definições do ProblemDetailsOptions, lá tem todas.
                options.TraceIdPropertyName = "Observability Id";
                //Como o nome diz, aqui será feita uma verificação para determinar se a exception details será colocada no no response               
                options.IncludeExceptionDetails = (Ctx, ex) =>
                {
                    var env = Ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsEnvironment("Development");
                };
            });

            return Services;
        }


        public static IServiceCollection AddAuthorizathionAndAuthentication(this IServiceCollection Services, IConfiguration Configuration)
        {
            
            Services.AddAuthentication(options =>
            {
                ConfigureTokenDefaultsBearer(options);                
            })
           .AddJwtBearer(options =>
           {
               ConfigureTokenValidation(options, Configuration);
           });
            Services.AddAuthorization();

            return Services;
        }
        //Configurações da Autenticação e validação do token
        public static void ConfigureTokenDefaultsBearer(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
        public static void ConfigureTokenValidation(JwtBearerOptions options, IConfiguration Configuration)
        {
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetRequiredSection("Jwt:SecretKey").Value))
            };
        }
            
        
        public static IServiceCollection AddCorsSupport(this IServiceCollection Services)
        {
            Services.AddCors(options =>
            {
                ConfigurePolicyManagerCors(options);
            });
            return Services;
        }
        //Configuração das policies no cors
        public static void ConfigurePolicyManagerCors(CorsOptions options)
        {
            options.AddPolicy("Dev", configurePolicy =>
            {
                configurePolicy.WithOrigins()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin();
            });
            options.AddPolicy("Prod", configurePolicy =>
            {

                configurePolicy.WithOrigins("http://localhost:8000")
                .WithMethods("GET", "PUT", "POST", "PATCH", "DELETE")
                .AllowAnyHeader()             
                .AllowCredentials();

            });
        }
    }
    
}
