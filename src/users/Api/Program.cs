using System.Text;
using Api.Authorization.Handlers;
using Api.Authorization.Requirements;
using Api.Filters;
using Api.Validators;
using Application.Mapping;
using Application.Services;
using Data.Contexts;
using Domain.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Dtos;
using Shared.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(opts => opts.Filters.Add<ExceptionHandleFilter>()).ConfigureApiBehaviorOptions(opts =>
    opts.InvalidModelStateResponseFactory = ctx => new BadRequestObjectResult(ctx.ModelState.Values.First(q => q.Errors.Count > 0).Errors
        .First(er => !string.IsNullOrEmpty(er.ErrorMessage)).ErrorMessage));

#region Validators
builder.Services.AddValidatorsFromAssemblyContaining<RequisiteCreateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
#endregion
#region Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = false, ValidateAudience = false, ValidateIssuer = false,
        ValidateLifetime = false, ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"])),
    };
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
builder.Services.AddSingleton<IAuthorizationHandler, RequireSameUserHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizeRequisiteUserHandler>();
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("SameUserPolicy", policy => policy.AddRequirements(new SameUserRequirement()));
    opts.AddPolicy("RequisiteUserPolicy", policy => policy.AddRequirements(new RequisiteUserRequirement()));
});
#endregion
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddDbContext<ApplicationContext>(opts => opts.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequisiteService, RequisiteService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddAutoMapper(typeof(UserProfile), typeof(RequisiteProfile));
builder.Services.AddEndpointsApiExplorer();
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