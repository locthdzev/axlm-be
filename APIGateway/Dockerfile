# Bước 1: Sử dụng .NET SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Đặt thư mục làm việc trong Docker container
WORKDIR /source
EXPOSE 8080

# Copy toàn bộ file .csproj của các project vào Docker container
COPY ./APIGateway/APIGateway.csproj ./APIGateway/

# Restore các dependencies cho các project
RUN dotnet restore ./APIGateway/APIGateway.csproj

# Copy toàn bộ source code vào Docker container
COPY ./APIGateway/ ./APIGateway/


# Build project API_Customer
RUN dotnet build ./APIGateway/APIGateway.csproj -c Release -o /app/build

# Bước 2: Sử dụng .NET runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .

# Chạy ứng dụng khi container khởi động
ENTRYPOINT ["dotnet", "APIGateway.dll"]
