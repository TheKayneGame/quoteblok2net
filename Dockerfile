#Builder
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
COPY ./ ./
RUN dotnet publish "./quoteblok2net.csproj" -c Release -r linux-x64 -p:PublishSingleFile=true

#Runner
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /source/bin/Release/net5.0/linux-x64/publish /app/

CMD [ "./quoteblok2net" ]
