using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using ApiGateway;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
var routesConfig = new ConfigurationBuilder().AddJsonFile("routes.json").Build();
// Add services to the container.
using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
builder.Services.AddCors(b => b.AddDefaultPolicy(c => c.SetIsOriginAllowed(_ => true).AllowCredentials().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddOcelot(routesConfig);
var app = builder.Build();
// Configure the HTTP request pipeline.
var logger = loggerFactory.CreateLogger<Program>();
string refreshPath = builder.Configuration["AuthSettings:RefreshTokenPath"];
string authPath = builder.Configuration["AuthSettings:AuthPath"];
string secretKey = builder.Configuration["AuthSettings:SecretKey"];
app.UseCors();
app.UseWebSockets();
await app.UseOcelot(new OcelotPipelineConfiguration
{
    AuthorizationMiddleware = async (context, next) =>
    {
        if (context.Request.Headers.Any(h => h.Key == "Role"))
        {
            logger.LogInformation("Request requires authorization: Header contains role");
            var accessToken = context.Request.Cookies["jwt-access"];
            var status = AuthorizationError.None;
            if (!string.IsNullOrEmpty(accessToken))
            {
                var headersRoles = context.Request.Headers["Role"].ToString().Split(';');
                logger.LogInformation("Access token from cookies: {AccessToken}, roles: {Role}", 
                    accessToken, headersRoles.Aggregate("", (current, r) => current + r));
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(accessToken))
                {
                    logger.LogInformation("Token is readable");
                    var parameters = new TokenValidationParameters
                    {
                        ValidateActor = false, ValidateIssuer = false,
                        ValidateLifetime = false, ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                    logger.LogInformation("Signature: {SecretKey}", secretKey);
                    SecurityToken? validatedToken;
                    try
                    {
                        handler.ValidateToken(accessToken, parameters, out validatedToken);
                    }
                    catch
                    {
                        validatedToken = null;
                    }
                    if (validatedToken != null)
                    {
                        logger.LogInformation("Valid token signature");
                        var user = validatedToken as JwtSecurityToken;
                        var role = user?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                        if (role != null && headersRoles.Any(r => r == role))
                        {
                            logger.LogInformation("Role is valid from token: {Role}", role);
                            var exp = user?.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;
                            if (exp != null)
                            {
                                logger.LogInformation("Found expiry date in token: {Exp}", exp);
                                var leftExpiryTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
                                if (leftExpiryTime < DateTime.Now || leftExpiryTime - DateTime.Now <= TimeSpan.FromMinutes(2))
                                {
                                    logger.LogWarning("Token needs to be refreshed");
                                    var cookies = new CookieContainer();
                                    var baseAddress = new Uri(authPath);
                                    string? refreshToken = context.Request.Cookies["jwt-refresh"];
                                    logger.LogInformation("Old refresh token: {RefreshToken}", refreshToken);
                                    using var clientHandler = new HttpClientHandler { CookieContainer = cookies };
                                    cookies.Add(baseAddress, new Cookie("jwt-access", accessToken));
                                    cookies.Add(baseAddress, new Cookie("jwt-refresh", refreshToken));
                                    using var client = new HttpClient(clientHandler){ BaseAddress = baseAddress };
                                    var request = new HttpRequestMessage(HttpMethod.Patch, refreshPath);
                                    request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + accessToken);
                                    request.Headers.Add("Refresh", refreshToken);
                                    using var response = await client.SendAsync(request);
                                    if (response.IsSuccessStatusCode) 
                                        logger.LogInformation("Refreshed token");
                                    else
                                    {
                                        logger.LogError(@"Failed to refresh: \n response url: {ResponseUrl}; \n
                                                        response message: {ReasonPhrase}, {StatusCode}",
                                            response.RequestMessage?.RequestUri, response.ReasonPhrase,
                                            response.StatusCode);
                                        context.Response.StatusCode = 500;
                                    }
                                }
                                logger.LogInformation("Successful authorization");
                                await next();
                            } else{ logger.LogError("Token does not have expiration date"); status = AuthorizationError.Unauthorized;}
                        } else{ logger.LogError("Token role is not authorized"); status = AuthorizationError.Forbidden;}
                    } else{ logger.LogError("Invalid token signature"); status = AuthorizationError.Unauthorized;}
                } else {logger.LogError("Token is invalid"); status = AuthorizationError.Unauthorized;}
            } else {logger.LogError("Token is not found in cookies"); status = AuthorizationError.Unauthorized; }

            switch (status)
            {
                case AuthorizationError.Unauthorized:
                    context.Items.SetError(new UnauthorizedError("Unauthorized"));
                    break;
                case AuthorizationError.Forbidden:
                    context.Items.SetError(new UserDoesNotHaveClaimError(""));
                    break;
            }
        }
        else
        {
            logger.LogInformation("Request for unauthorized resource");
            await next();
        }
    }
});
app.Run();