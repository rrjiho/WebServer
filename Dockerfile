# 빌드용 SDK 이미지
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet publish ./ServerAPI/ServerAPI.csproj -c Release -o /app/publish

# 런타임 이미지
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ServerAPI.dll"]