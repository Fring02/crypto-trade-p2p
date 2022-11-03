using System.Text;
using AuthService;
using AuthService.Configuration;
using AuthService.Dtos;
using AuthService.Filters;
using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
#region Controllers
builder.Services.AddControllers(opts => opts.Filters.Add<ExceptionHandleFilter>()).ConfigureApiBehaviorOptions(opts => 
    opts.InvalidModelStateResponseFactory = ctx => new BadRequestObjectResult(ctx.ModelState.Values.First().Errors.First().ErrorMessage));
#endregion
#region Config
builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection("AuthOptions"));
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
//load .env
var envPath = builder.Environment.IsDevelopment() ? ".env" : ".prod.env";
builder.Configuration.LoadEnv(Path.Combine(Directory.GetCurrentDirectory(), envPath));
#endregion
#region Application Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
#endregion
#region Mapping
builder.Services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(c => c.CreateMap<RegisterDto, User>())));
#endregion
#region Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidateIssuer = false, ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthOptions:SecretKey"]))
    };
    opts.IncludeErrorDetails = true;
});
builder.Services.AddSingleton(new CookieOptions { HttpOnly = true, IsEssential = true });
#endregion
#region MassTransit
builder.Services.AddMassTransit(opts => opts.UsingRabbitMq((_, config) =>
{
    config.Host(builder.Configuration["AMQP_HOST"], ushort.Parse(builder.Configuration["AMQP_PORT"]), 
        builder.Configuration["AMQP_USERNAME"], mqConfig =>
    {
        mqConfig.Username(builder.Configuration["AMQP_USERNAME"]);
        mqConfig.Password(builder.Configuration["AMQP_PASSWORD"]);
    });
}));
builder.Services.AddScoped<IWalletProducerService, WalletsProducerService>();
#endregion
#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion
var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();