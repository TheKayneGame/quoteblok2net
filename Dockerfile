FROM alpine:3.12
VOLUME [ "/config" ]
RUN apk add bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib curl
RUN apk add libgdiplus --repository https://dl-3.alpinelinux.org/alpine/edge/testing/
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel 5.0 -InstallDir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
COPY . /app
RUN  dotnet publish /app/quoteblok2net.csproj -c Release

WORKDIR /config
CMD ["sh" ""]
