FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /source

COPY . . 

RUN dotnet restore "./PollApp.Api/PollApp.Api.csproj" --disable-parallel
RUN dotnet publish "./PollApp.Api/PollApp.Api.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5000

ENTRYPOINT [ "dotnet", "PollApp.Api.dll" ]