global using static Data.Extensions.ObjectIdExtension;
using System.Reflection;
using Application.Responses.Ethereum.Wallets;
using Data.Configuration;
using Data.Repositories.Wallets;
using Domain.Configuration;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using FluentValidation.AspNetCore;
using Infrastructure.MessageBroker.Consumers;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Wallets.Api.Extensions;
using Wallets.Api.Filters;
using Wallets.Api.Validators;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
#region FluentValidation, Controllers
builder.Services.AddControllers(opts => opts.Filters.Add<ExceptionHandleFilter>())
.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateEthereumWalletValidator>())
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
builder.Configuration.AddEnvironmentVariables();
//builder.Configuration.LoadEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
#endregion
#region Data
builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration["DatabaseSettings:ConnectionString"]));
builder.Services.AddScoped<IEthereumWalletsRepository<ObjectId>, EthereumWalletsRepository>();
builder.Services.AddScoped<IEthereumP2PWalletsRepository<ObjectId>, EthereumP2PWalletsRepository>();
#endregion
#region MassTransit
builder.Services.AddMassTransit(opts =>
{
    opts.AddConsumer<CreateWalletMessagesConsumer>();
    opts.AddConsumer<LoadWalletsMessagesConsumer>();
    opts.UsingRabbitMq((ctx, config) =>
    {
        config.Host(builder.Configuration["AMQP_HOST"], ushort.Parse(builder.Configuration["AMQP_PORT"]),
            builder.Configuration["AMQP_USERNAME"], mqConfig =>
            {
                mqConfig.Username(builder.Configuration["AMQP_USERNAME"]);
                mqConfig.Password(builder.Configuration["AMQP_PASSWORD"]);
            });
        config.ConfigureEndpoints(ctx);
    });
});
#endregion
#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Crypto wallets API: Ethereum wallets",
        Description = "API for managing Ethereum wallets: create, load existing ones and get information about wallets"
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
}
app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseCors(b => b.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseAuthorization();
app.MapControllers();
app.Run();