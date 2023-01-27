#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sending.Results.Example/Sending.Results.Example.csproj", "Sending.Results.Example/"]
COPY ["Sending.Core.Example/Sending.Core.Example.csproj", "Sending.Core.Example/"]
COPY ["SendingScheduler.Core/SendingScheduler.Core.csproj", "SendingScheduler.Core/"]
COPY ["SendingScheduler.Results/SendingScheduler.Results.csproj", "SendingScheduler.Results/"]
RUN dotnet restore "Sending.Results.Example/Sending.Results.Example.csproj"
COPY . .
WORKDIR "/src/Sending.Results.Example"
RUN dotnet build "Sending.Results.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sending.Results.Example.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sending.Results.Example.dll"]