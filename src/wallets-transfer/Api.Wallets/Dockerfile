FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Api.Wallets/Api.Wallets.csproj", "Api.Wallets/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "Api.Wallets/Api.Wallets.csproj"
COPY . .
WORKDIR "/src/Api.Wallets"
RUN dotnet build "Api.Wallets.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.Wallets.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.Wallets.dll"]