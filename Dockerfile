# 빌드용 SDK 이미지
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# dotnet ef 설치 (추가)
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# 프로젝트 복사
COPY . .

# 솔루션 복사 및 복합 빌드
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# 런타임 이미지
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 컨테이너 시작 시 실행할 명령어
ENTRYPOINT ["dotnet", "ServerAPI.dll"]