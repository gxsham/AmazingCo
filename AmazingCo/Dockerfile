FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AmazingCo/AmazingCo.csproj", "AmazingCo/"]
RUN dotnet restore "AmazingCo/AmazingCo.csproj"
COPY . .
WORKDIR "/src/AmazingCo"
RUN dotnet build "AmazingCo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AmazingCo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AmazingCo.dll"]
