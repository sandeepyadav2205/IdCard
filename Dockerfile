# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["IcardProject.csproj", "."]
RUN dotnet restore "IcardProject.csproj"

# Copy all project files
COPY . .

# Build application
RUN dotnet build "IcardProject.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "IcardProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose ports
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost/Student/Index || exit 1

# Entry point
ENTRYPOINT ["dotnet", "IcardProject.dll"]
