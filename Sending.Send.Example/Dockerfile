#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sending.Send.Example/Sending.Send.Example.csproj", "Sending.Send.Example/"]
COPY ["Sending.Core.Example/Sending.Core.Example.csproj", "Sending.Core.Example/"]
COPY ["SendingScheduler.Core/SendingScheduler.Core.csproj", "SendingScheduler.Core/"]
COPY ["SendingScheduler.Send/SendingScheduler.Send.csproj", "SendingScheduler.Send/"]
RUN dotnet restore "Sending.Send.Example/Sending.Send.Example.csproj"
COPY . .
WORKDIR "/src/Sending.Send.Example"
RUN dotnet build "Sending.Send.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sending.Send.Example.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sending.Send.Example.dll"]