# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["IcardProject/IcardProject.csproj", "IcardProject/"]

# Restore dependencies
RUN dotnet restore "IcardProject/IcardProject.csproj"

# Copy source code
COPY ["IcardProject/", "IcardProject/"]

# Build the project
WORKDIR "/src/IcardProject"
RUN dotnet build "IcardProject.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "IcardProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published app from publish stage
COPY --from=publish /app/publish .

# Expose portdo
EXPOSE 80
EXPOSE 443

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80

# Start the application
ENTRYPOINT ["dotnet", "IcardProject.dll"]