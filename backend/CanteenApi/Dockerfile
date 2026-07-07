FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CanteenApi.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "CanteenApi.dll"]
