# 1단계: 빌드 및 EF 실행 환경
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY . .

RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet publish -c Release -o /app/publish  # ✅ 이 줄 추가

# 2단계: 런타임 이미지
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .                # ✅ 경로 변경됨
ENTRYPOINT ["dotnet", "ServerAPI.dll"]