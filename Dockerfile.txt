# STEP 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o out

# STEP 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/out ./

# Expose default ports
EXPOSE 80
EXPOSE 443

# Run the app
ENTRYPOINT ["dotnet", "CpiService.dll"]
