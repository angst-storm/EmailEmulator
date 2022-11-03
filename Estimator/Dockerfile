FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY Estimator/Estimator.csproj .
COPY Shared/Shared.csproj .
RUN dotnet restore "Estimator.csproj"
COPY . .
RUN dotnet build "Estimator.csproj" -c Release

FROM build AS publish
RUN dotnet publish "Estimator.csproj" -c Release -o /publish

FROM base AS final
WORKDIR /app
COPY --from=publish /publish .
ENTRYPOINT ["dotnet", "Estimator.dll"]
