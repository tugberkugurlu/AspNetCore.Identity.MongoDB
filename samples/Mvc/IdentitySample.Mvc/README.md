## How to Run

You need to have MongoDB exposed through `127.0.0.1:27017`. If not, you can get it up through [Docker](https://www.docker.com/):

```bash
docker run --name some-mongo -d -p "27017:27017" mongo
```

After that, you can run the application with below commands:

```bash
dotnet restore
dotnet run
```