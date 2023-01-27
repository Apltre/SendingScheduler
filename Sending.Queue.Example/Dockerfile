#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sending.Queue.Example/Sending.Queue.Example.csproj", "Sending.Queue.Example/"]
COPY ["Sending.Core.Example/Sending.Core.Example.csproj", "Sending.Core.Example/"]
COPY ["SendingScheduler.Core/SendingScheduler.Core.csproj", "SendingScheduler.Core/"]
COPY ["SendingScheduler.Queue/SendingScheduler.Queue.csproj", "SendingScheduler.Queue/"]
RUN dotnet restore "Sending.Queue.Example/Sending.Queue.Example.csproj"
COPY . .
WORKDIR "/src/Sending.Queue.Example"
RUN dotnet build "Sending.Queue.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sending.Queue.Example.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sending.Queue.Example.dll"]