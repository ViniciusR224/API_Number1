using API_Number1.AppDbContext;
using API_Number1.Filters;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IRepository_Base;
using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Middlewares;
using API_Number1.Repositories.Repository_Base;
using API_Number1.Repositories.User_Repository;
using API_Number1.Services.FactoryBase;
using API_Number1.Services.Jwt_Service;
using API_Number1.Services.PasswordHasher_Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace API_Number1.DependencyInjectionConfig
{
    public static class DependencyInjection
    {
        
        //Classe para configuração dos Services na startup - Funcinará como uma extension - Forma de estruturar DI
        public static IServiceCollection AddControllerAndSweggerSupport(this IServiceCollection Services)
        {
            ////Estou criando uma policy aqui porque fazer no AddAuthorization seria mais para uma situação global, não somente nos controllers
            ////Fica aqui configurado que todos os usuarios devem estar authenticados para conseguir a authorização de usar os métodos
            Services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole("User", "Administrator").Build();
                //Exceptions nivel controller

                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add<ExceptionFilterController>();

            }).AddNewtonsoftJson();

            // Adicionar suporte ao Swagger para documentação da API
            Services.AddSwaggerGen();


            return Services;
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
            Services.AddScoped<IPasswordHasher, PasswordHasherService>();
            Services.AddScoped(typeof(IRepostoryBase<>), typeof(RepositoryBase<>));
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<IJwtService, JwtGeneratorSevice>();
            Services.AddScoped<IServiceBase, BaseService>();
            Services.AddTransient<ExceptionMiddleware>();

            Services.AddCors(options =>
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
                    configurePolicy.WithOrigins("https://localhost:7077/swagger/index.html")
                    .WithHeaders("Production")
                    .WithMethods("GET", "PUT", "PATCH", "DELETE");
                });
            });
            return Services;
        }


        public static IServiceCollection AddAuthorizathionAndAuthentication(this IServiceCollection Services, IConfiguration Configuration)
        {
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
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetRequiredSection("Jwt:SecretKey").Value))
               };
           });
            Services.AddAuthorization();

            return Services;
        }




    }
}
