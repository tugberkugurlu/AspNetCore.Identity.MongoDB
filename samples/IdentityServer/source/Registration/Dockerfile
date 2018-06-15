FROM microsoft/dotnet:2.1-sdk
ENV NUGET_XMLDOC_NODE skip

WORKDIR /vsdbg
RUN update-ca-certificates --fresh \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        unzip \
    && rm -rf /var/lib/apt/lists* \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

WORKDIR /app

EXPOSE 5002

ENV ASPNETCORE_URLS http://+:5002

ENTRYPOINT [ "tail", "-f", "/dev/null" ]