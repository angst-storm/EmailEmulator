FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ../Shared/Shared.csproj Shared/
COPY /Estimator/Estimator.csproj Estimator/
RUN dotnet restore "Estimator/Estimator.csproj"
COPY . .
WORKDIR "/src/Estimator"
RUN dotnet build "Estimator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Estimator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Estimator.dll"]
