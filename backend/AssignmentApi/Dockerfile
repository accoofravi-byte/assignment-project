FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

# Ensure the app listens on the container network interface
ENV ASPNETCORE_URLS=http://+:8080

# Install curl for HEALTHCHECK (small runtime cost) and remove apt lists
RUN apt-get update \
	&& apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*

# Docker healthcheck uses the app health endpoint
HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
	CMD curl -f http://localhost:8080/healthz || exit 1

ENTRYPOINT ["dotnet", "AssignmentApi.dll"]