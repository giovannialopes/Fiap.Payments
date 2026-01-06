# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Payments.Api/Payments.Api.csproj", "Payments.Api/"]
COPY ["Payments.Domain/Payments.Domain.csproj", "Payments.Domain/"]
COPY ["Payments.Infrastructure/Payments.Infrastructure.csproj", "Payments.Infrastructure/"]
COPY ["Payments.Domain.Shared/Payments.Domain.Shared.csproj", "Payments.Domain.Shared/"]

RUN dotnet restore "Payments.Api/Payments.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Payments.Api"
RUN rm -rf obj bin && dotnet publish "Payments.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=build /app/publish .

# Set ownership and permissions
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Payments.Api.dll"]
