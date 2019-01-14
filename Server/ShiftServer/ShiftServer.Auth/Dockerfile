FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["ShiftServer.Auth/ShiftServer.Auth.csproj", "ShiftServer.Auth/"]
RUN dotnet restore "ShiftServer.Auth/ShiftServer.Auth.csproj"
COPY . .
WORKDIR "/src/ShiftServer.Auth"
RUN dotnet build "ShiftServer.Auth.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ShiftServer.Auth.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShiftServer.Auth.dll"]