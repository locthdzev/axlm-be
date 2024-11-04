# Bước 1: Sử dụng .NET SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Đặt thư mục làm việc trong Docker container
WORKDIR /source
EXPOSE 8080

# Copy toàn bộ file .csproj của các project vào Docker container
COPY ./Repositories/Repositories.csproj ./Repositories/
COPY ./Data/Data.csproj ./Data/
COPY ./UserAPIServices/UserAPIServices.csproj ./UserAPIServices/

# Restore các dependencies cho các project
RUN dotnet restore ./UserAPIServices/UserAPIServices.csproj

# Copy toàn bộ source code vào Docker container
COPY ./Repositories/ ./Repositories/
COPY ./Data/ ./Data/
COPY ./UserAPIServices/ ./UserAPIServices/

# Copy toàn bộ thư mục TemplateEmail vào Docker container
COPY ./Data/Utilities/TemplateEmail/ /Data/Utilities/TemplateEmail/
# Copy toàn bộ thư mục Utilities vào Docker container
COPY ./Data/Utilities/ /Data/Utilities/


# Build project API_Customer
RUN dotnet build ./UserAPIServices/UserAPIServices.csproj -c Release -o /app/build

# Bước 2: Sử dụng .NET runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .

# Chạy ứng dụng khi container khởi động
ENTRYPOINT ["dotnet", "UserAPIServices.dll"]
