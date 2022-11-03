using System.Text;
using Api;
using Api.Authorization.Handlers;
using Api.Authorization.Requirements;
using Api.Validators;
using Application.Mapping;
using Application.Services;
using Data.Contexts;
using Data.Repositories;
using Domain.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Shared.Interfaces.Repositories;
using Shared.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(opts => opts.Filters.Add<ExceptionHandleFilter>()).ConfigureApiBehaviorOptions(opts => 
    opts.InvalidModelStateResponseFactory = ctx => new BadRequestObjectResult(ctx.ModelState.Values.First(q => q.Errors.Count > 0).Errors.First(er => !string.IsNullOrEmpty(er.ErrorMessage)).ErrorMessage));
builder.Services.AddValidatorsFromAssemblyContaining<LotCreateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
#region Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = false, ValidateAudience = false, ValidateIssuer = false,
        ValidateLifetime = false, ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"])),
    };
    opts.IncludeErrorDetails = true;
    opts.Events = new JwtBearerEvents()
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
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizeCreateLotOwnerHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizeCheckLotOwnerHandler>();
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("CreateLotOwnerPolicy", policy => policy.AddRequirements(new CreateLotOwnerRequirement()));
    opts.AddPolicy("CheckLotOwnerPolicy", policy => policy.AddRequirements(new CheckLotOwnerRequirement()));
});
#endregion
builder.Services.AddAutoMapper(typeof(LotsProfile));
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddScoped<ILotsRepository, LotsRepository>();
builder.Services.AddScoped<ILotsService, LotsService>();

builder.Services.AddSingleton(new RestClient(builder.Configuration["ConnectionStrings:RequisitesUrl"]));
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();