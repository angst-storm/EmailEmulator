FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ../Shared/Shared.csproj Shared/
COPY /Analyzer/Analyzer.csproj Analyzer/
RUN dotnet restore "Analyzer/Analyzer.csproj"
COPY . .
WORKDIR "/src/Analyzer"
RUN dotnet build "Analyzer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Analyzer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Analyzer.dll"]
