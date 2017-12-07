FROM microsoft/dotnet:2-sdk

COPY . /app/
WORKDIR /app/

RUN dotnet restore
RUN dotnet publish -c Debug -o out

EXPOSE 5001
ENTRYPOINT [ "dotnet", "out/mvcclient.dll" ]