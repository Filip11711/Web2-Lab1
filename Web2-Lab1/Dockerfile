# Use the official ASP.NET Core SDK image as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory to /app
WORKDIR /app

# Copy the project files to the container
COPY . .

# Build the ASP.NET Core application
RUN dotnet publish -c Release -o /app/out

# Use the official ASP.NET Core Runtime image as the base image for running
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Set the working directory to /app
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose port 80 for the application to listen on
EXPOSE 80

# Define environment variables if necessary
# ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS=http://*:80

# Start the ASP.NET Core application
CMD ["dotnet", "Web2-Lab1.dll"]