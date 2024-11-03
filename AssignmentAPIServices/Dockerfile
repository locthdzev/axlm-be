# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AssignmentAPIServices/AssignmentAPIServices.csproj", "AssignmentAPIServices/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
RUN dotnet restore "AssignmentAPIServices/AssignmentAPIServices.csproj"
COPY . .
WORKDIR "/src/AssignmentAPIServices"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssignmentAPIServices.dll"]
