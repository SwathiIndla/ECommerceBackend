# Use the .NET Core SDK as the base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj and restore dependencies (optimizing caching)
COPY /Ecommerce/program.cs /app/
RUN dotnet restore

# Copy the remaining application code
COPY . ./

# Build the application inside the container
RUN dotnet publish -c Release -o out

# Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80

# Define the entry point for the application
ENTRYPOINT ["dotnet", "YourBackendApplicationName.dll"]
