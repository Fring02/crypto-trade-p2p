global using static Data.Extensions.ObjectIdExtension;
using System.Reflection;
using Application.Responses.Ethereum.Wallets;
using Domain.Configuration;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using Api.Transfer.Filters;
using Api.Transfer.Validators.Ethereum;
using Data.Configuration;
using Data.Repositories.Wallets;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region FluentValidation, Controllers
builder.Services.AddControllers(opts => {
     opts.Filters.Add<ExceptionHandleFilter>();
 }).AddNewtonsoftJson().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RefundEtherFromP2PWalletValidator>())
 .ConfigureApiBehaviorOptions(opt => {
     opt.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState.Values.First(q => q.Errors.Count > 0).Errors.First(er => !string.IsNullOrEmpty(er.ErrorMessage)).ErrorMessage);
 });
#endregion
#region MediatR
builder.Services.AddMediatR(typeof(EthereumWalletResponse).Assembly);
#endregion
#region Logging
builder.Logging.AddConsole();
builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.RequestPath);
#endregion
#region ETH Accounts Manager
builder.Services.AddSingleton<EthereumAccountManager>();
#endregion
#region Configuration
builder.Services.Configure<BlockchainConnections>(builder.Configuration.GetSection("BlockchainConnections"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<BlockchainConnections>>().Value);
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoSettings>>().Value);
#endregion
#region Data
builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration["DatabaseSettings:ConnectionString"]));
builder.Services.AddScoped<IEthereumWalletsRepository<ObjectId>, EthereumWalletsRepository>();
builder.Services.AddScoped<IEthereumP2PWalletsRepository<ObjectId>, EthereumP2PWalletsRepository>();
#endregion
#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2", Title = "ETH Transfer API",
        Description = "API for transferring Ethereum cryptocurrency via smart contracts: block sum, transfer and revert transfer"
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else 
    app.UseHsts();
app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseCors(b => b.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseAuthorization();
app.MapControllers();
app.Run();