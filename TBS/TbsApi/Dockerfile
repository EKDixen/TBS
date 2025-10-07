# Multi-stage build for TbsApi (.NET)
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY *.sln ./
COPY TbsApi/*.csproj TbsApi/
RUN dotnet restore TbsApi/TbsApi.csproj

# Copy source and publish
COPY TbsApi/ TbsApi/
WORKDIR /src/TbsApi
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy build artifacts
COPY --from=build /app/out .

# Render provides $PORT; bind to all interfaces
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "TbsApi.dll"]
