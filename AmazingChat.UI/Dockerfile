#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AmazingChat.UI/AmazingChat.UI.csproj", "AmazingChat.UI/"]
COPY ["AmazingChat.Application/AmazingChat.Application.csproj", "AmazingChat.Application/"]
COPY ["AmazingChat.Domain/AmazingChat.Domain.csproj", "AmazingChat.Domain/"]
COPY ["AmazingChat.Domain.Shared/AmazingChat.Domain.Shared.csproj", "AmazingChat.Domain.Shared/"]
COPY ["AmazingChat.Infra.CrossCutting.Services/AmazingChat.Infra.CrossCutting.Services.csproj", "AmazingChat.Infra.CrossCutting.Services/"]
COPY ["AmazingChat.Infra.Data/AmazingChat.Infra.Data.csproj", "AmazingChat.Infra.Data/"]
COPY ["AmazingChat.Infra.CrossCutting.Configurations/AmazingChat.Infra.CrossCutting.Configurations.csproj", "AmazingChat.Infra.CrossCutting.Configurations/"]
COPY ["AmazingChat.Infra.CrossCutting.IoC/AmazingChat.Infra.CrossCutting.IoC.csproj", "AmazingChat.Infra.CrossCutting.IoC/"]
RUN dotnet restore "AmazingChat.UI/AmazingChat.UI.csproj"
COPY . .
WORKDIR "/src/AmazingChat.UI"
RUN dotnet build "AmazingChat.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AmazingChat.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AmazingChat.UI.dll"]