#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8888

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PowerplantCodingChallenge.API/PowerplantCodingChallenge.API.csproj", "PowerplantCodingChallenge.API/"]
RUN dotnet restore "PowerplantCodingChallenge.API/PowerplantCodingChallenge.API.csproj"
COPY . .
WORKDIR "/src/PowerplantCodingChallenge.API"
RUN dotnet build "PowerplantCodingChallenge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PowerplantCodingChallenge.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerplantCodingChallenge.API.dll"]