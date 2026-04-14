# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

COPY ["SistemaMedico.Web/SistemaMedico.Web.csproj", "SistemaMedico.Web/"]
COPY ["SistemaMedico.Application/SistemaMedico.Application.csproj", "SistemaMedico.Application/"]
COPY ["SistemaMedico.Infrastructure/SistemaMedico.Infrastructure.csproj", "SistemaMedico.Infrastructure/"]
COPY ["SistemaMedico.Domain/SistemaMedico.Domain.csproj", "SistemaMedico.Domain/"]

RUN dotnet restore "SistemaMedico.Web/SistemaMedico.Web.csproj"

COPY . .

RUN dotnet publish "SistemaMedico.Web/SistemaMedico.Web.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=builder /app/publish .

EXPOSE 10000

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SistemaMedico.Web.dll"]