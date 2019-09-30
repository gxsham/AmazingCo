FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AmazingCo.ApiGW/AmazingCo.ApiGW.csproj", "AmazingCo.ApiGW/"]
RUN dotnet restore "AmazingCo.ApiGW/AmazingCo.ApiGW.csproj"
COPY . .
WORKDIR "/src/AmazingCo.ApiGW"
RUN dotnet build "AmazingCo.ApiGW.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AmazingCo.ApiGW.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AmazingCo.ApiGW.dll"]
