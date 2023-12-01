global using PollApp.Api.Models;
global using PollApp.Api.Dtos;
global using PollApp.Api.Services;
global using Microsoft.EntityFrameworkCore;
global using PollApp.Api.Data;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        // Cors
        // var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy(name: myAllowSpecificOrigins,
        //                       policy  =>
        //                       {
        //                           policy.WithOrigins("http://localhost:5173");
        //                       });
        // });


        // Add services to the container.

        builder.Services.AddDbContext<DataContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"));
        }
            // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        builder.Services.AddAuthentication().AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters{
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!))
            };
        });

        // Add auto mappers
        builder.Services.AddAutoMapper(typeof(Program).Assembly);

        // Add custom services
        builder.Services.AddScoped<IPollService, PollService>();
        builder.Services.AddScoped<IUserService, UserService>();
        // builder.Services.AddSingleton<IPollService, PollService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        // {
            app.UseSwagger();
            app.UseSwaggerUI();
        // }

        app.UseHttpsRedirection();

        // app.Use((ctx, next) => {
        //     ctx.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:5173";
        //     return next();
        // });

        // app.UseCors(myAllowSpecificOrigins);

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}