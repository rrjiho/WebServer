FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY . .

RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet publish ./ServerAPI/ServerAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServerAPI.dll"]