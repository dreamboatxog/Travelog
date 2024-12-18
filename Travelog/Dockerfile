# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Travelog/Travelog.csproj", "Travelog/"]
COPY ["Travelog.Application/Travelog.Application.csproj", "Travelog.Application/"]
COPY ["Travelog.Contracts/Travelog.Contracts.csproj", "Travelog.Contracts/"]
COPY ["Travelog.Core/Travelog.Core.csproj", "Travelog.Core/"]
COPY ["Travelog.DataAccess/Travelog.DataAccess.csproj", "Travelog.DataAccess/"]
RUN dotnet restore "./Travelog/Travelog.csproj"
COPY . .
WORKDIR "/src/Travelog"
RUN dotnet build "./Travelog.csproj" -c Release -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
RUN dotnet publish "./Travelog.csproj" -c Release -o /app/publish 

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Travelog.dll"]