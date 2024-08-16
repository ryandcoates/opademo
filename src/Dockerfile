# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the project file and restore dependencies
COPY *.csproj ./

# Restore the project dependencies
RUN dotnet restore

# Copy the rest of the application files
COPY . .

# Build the application in Release mode
RUN dotnet publish -c Release -o /app/out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the previous stage
COPY --from=build /app/out .

# Expose the port the app will run on
EXPOSE 80

# Define the entry point for the container, using the application name
ENTRYPOINT ["dotnet", "opademo.dll"]
