# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the project files to the working directory
COPY . ./

# Restore the dependencies
RUN dotnet restore

# Build the project
RUN dotnet build -c Release -o out

# Publish the project
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build-env /app/out .

# Expose port 5107
EXPOSE 5107
# Start the application
ENTRYPOINT ["dotnet", "docker.k8.dll"]
