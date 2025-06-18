# ------------------------------------------------------
# 1단계: 빌드 및 dotnet ef 실행 환경 구성
# ------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# EF CLI 설치
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# 프로젝트 복사
COPY . .

# 복원 및 빌드 (EF 명령 실행용 바이너리 포함)
RUN dotnet restore
RUN dotnet build -c Release

# dotnet ef 명령 실행을 위해 이 이미지를 따로 유지 (추후 사용)
# 👉 따로 EF 명령 실행을 위한 도커 명령어로 실행할 예정

# ------------------------------------------------------
# 2단계: 런타임 이미지 (실제 서비스용)
# ------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 빌드 결과 복사
COPY --from=build /src/ServerAPI/bin/Release/net8.0/publish/ .

# 실행 명령
ENTRYPOINT ["dotnet", "ServerAPI.dll"]