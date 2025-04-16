# Runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HTML_FC/HTML_FC.csproj HTML_FC/
COPY LIBCORE/LIBCORE.csproj LIBCORE/
RUN dotnet restore HTML_FC/HTML_FC.csproj

COPY . .
WORKDIR /src/HTML_FC
RUN dotnet publish HTML_FC.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HTML_FC.dll"]
