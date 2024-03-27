FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ .
WORKDIR PlanningPoker/WebApi
RUN dotnet restore
RUN dotnet build -c Release

FROM build AS publish
RUN dotnet publish PlanningPoker.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV PORT=8080

CMD ASPNETCORE_URLS="http://*:$PORT" dotnet PlanningPoker.WebApi.dll
