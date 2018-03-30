# to build app in your machine.
# `cd Ftp2AzureCore`
# `dotnet  publish -c release --runtime liniux-x64`
# to build image
# `cd ..`
# `docker -t xxx/xxx:tag . `
FROM microsoft/dotnet:2.0.6-runtime-deps

WORKDIR /app

COPY Ftp2AzureCore/bin/release/netcoreapp2.0/linux-x64/publish/* /app/

ENTRYPOINT ["./Ftp2AzureCore"]
