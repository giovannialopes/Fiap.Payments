FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Payments.Api/Payments.Api.csproj", "Payments.Api/"]
RUN dotnet restore "Payments.Api/Payments.Api.csproj"
COPY . .
WORKDIR "/src/Payments.Api"
RUN dotnet publish "Payments.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Payments.Api.dll"]
