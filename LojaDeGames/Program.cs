
using FluentValidation;
using LojaDeGames.Data;
using LojaDeGames.Model;
using LojaDeGames.Security.Implements;
using LojaDeGames.Security;
using LojaDeGames.Service;
using LojaDeGames.Service.Implements;
using LojaDeGames.Validator;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LojaDeGames
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddNewtonsoftJson(
                    options => {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    }
                );

            //Conexão com o Banco de Dados
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            //Registar a Validação das Entidades
            builder.Services.AddTransient<IValidator<Produto>, ProdutoValidator>();
            builder.Services.AddTransient<IValidator<Categoria>, CategoriaValidator>();
            builder.Services.AddTransient<IValidator<User>, UserValidator>();

            //Registrar as Classes de Serviço
            builder.Services.AddScoped<IProdutoService, ProdutoService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddTransient<IAuthService, AuthService>();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(Settings.Secret);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options => {
                options.AddPolicy(name: "MyPolicy", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });


            // Adicionar a Validação do Token JWT

            


            var app = builder.Build();

            //Criar o Banco de Dados e as Tabelas
            using (var scope = app.Services.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("MyPolice");

            app.UseAuthentication();

            app.UseAuthorization();
            
            app.MapControllers();

            app.Run();
        }
    }
}