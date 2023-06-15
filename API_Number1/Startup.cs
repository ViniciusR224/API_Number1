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
