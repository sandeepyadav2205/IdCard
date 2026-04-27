# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["IcardProject/IcardProject.csproj", "IcardProject/"]
RUN dotnet restore "IcardProject/IcardProject.csproj"

# Copy all files
COPY . .

# Build
WORKDIR "/src/IcardProject"
RUN dotnet build "IcardProject.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "IcardProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "IcardProject.dll"]