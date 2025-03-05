# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET 9 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/out .

# Expose port 8080
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "RestaurantAdmin.dll"]
