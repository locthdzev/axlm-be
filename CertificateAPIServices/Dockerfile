# Bước 1: Sử dụng .NET SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Đặt thư mục làm việc trong Docker container
WORKDIR /source

# Copy toàn bộ file .csproj của các project vào Docker container
COPY ./Repositories/Repositories.csproj ./Repositories/
COPY ./Data/Data.csproj ./Data/
COPY ./CertificateAPIServices/CertificateAPIServices.csproj ./CertificateAPIServices/

# Restore các dependencies cho các project
RUN dotnet restore ./CertificateAPIServices/CertificateAPIServices.csproj

# Copy toàn bộ source code vào Docker container
COPY ./Repositories/ ./Repositories/
COPY ./Data/ ./Data/
COPY ./CertificateAPIServices/ ./CertificateAPIServices/

# Copy file ServiceAccountKey vào Docker container
COPY ./Data/axlm-2024-firebase.json ./Data/axlm-2024-firebase.json

# Build project API_Customer
RUN dotnet build ./CertificateAPIServices/CertificateAPIServices.csproj -c Release -o /app/build

# Bước 2: Sử dụng .NET runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
COPY --from=build /source/Data/axlm-2024-firebase.json ./Data/axlm-2024-firebase.json 

# Chạy ứng dụng khi container khởi động
ENTRYPOINT ["dotnet", "CertificateAPIServices.dll"]
