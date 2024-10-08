FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY . ./

RUN ["dotnet" ,"restore"]

RUN ["dotnet" ,"build" ,"-c" ,"Release", "-o", "out"]

RUN ["dotnet", "publish", "-c", "Release", "-o", "out"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0


COPY --from=build-env /app/out .

ENV PORT=8080

EXPOSE $PORT

ENTRYPOINT ["dotnet", "docker.k8.dll"]
