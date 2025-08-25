FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/logs && chmod 755 /app/logs

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Desafio-NEPEN/Desafio-NEPEN.csproj", "Desafio-NEPEN/"]
RUN dotnet restore "Desafio-NEPEN/Desafio-NEPEN.csproj"
COPY . .
WORKDIR "/src/Desafio-NEPEN"
RUN dotnet build "./Desafio-NEPEN.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Desafio-NEPEN.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desafio-NEPEN.dll"]
USER root
RUN chown -R 1000:1000 /app/logs
USER app
ENTRYPOINT ["dotnet", "Desafio-NEPEN.dll"]
