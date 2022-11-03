using System.Text;
using Api;
using Application.Handlers;
using Application.Services;
using Data.Contexts;
using Domain.Settings;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

#region MediatR, Controllers
builder.Services.AddControllers(opts => opts.Filters.Add<ExceptionHandleFilter>())
    .ConfigureApiBehaviorOptions(opt => {
        opt.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState.Values.First(q => q.Errors.Count > 0).Errors.First(er => !string.IsNullOrEmpty(er.ErrorMessage)).ErrorMessage);
    });
builder.Services.AddMediatR(typeof(BeginSessionHandler).Assembly);
#endregion
#region Logging
builder.Logging.AddConsole();
builder.Services.AddHttpLogging(o => 
    o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.RequestPath);
#endregion
#region DbContext
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
#endregion
#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion
#region Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = false, ValidateAudience = false, ValidateIssuer = false,
        ValidateLifetime = false, ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"])),
    };
    opts.IncludeErrorDetails = true;
    opts.Events = new JwtBearerEvents
    {
        OnChallenge = ctx =>
        {
            Console.WriteLine($"401 Unauthorized: {ctx.Error}, \n {ctx.ErrorDescription} \n Url: {ctx.ErrorUri}");
            Console.WriteLine("Secret key: " + builder.Configuration["SecretKey"]);
            if (ctx.AuthenticateFailure is not null) return Task.FromException(ctx.AuthenticateFailure!);
            Console.WriteLine("Authentication failure is null");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine($"Authentication failure: {ctx.Exception.Message}");
            Console.WriteLine("Secret key: " + builder.Configuration["SecretKey"]);
            return Task.CompletedTask;
        },
    };
});
#endregion
#region Hangfire
builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();
#endregion
builder.Services.AddHttpClient();
builder.Services.AddScoped<SessionNotificationService>();
builder.Services.AddScoped<SessionManager>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration["RedisUrl"]));
builder.Services.Configure<SessionSettings>(builder.Configuration.GetSection("SessionSettings"));
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();
app.Run();