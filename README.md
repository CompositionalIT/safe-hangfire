# SAFE Hangfire

This template can be used to generate a full-stack web application using the [SAFE Stack](https://safe-stack.github.io/). It was created using the dotnet [SAFE Template](https://safe-stack.github.io/docs/template-overview/). If you want to learn more about the template why not start with the [quick start](https://safe-stack.github.io/docs/quickstart/) guide?

## Install pre-requisites

You'll need to install the following pre-requisites in order to build SAFE applications

* [.NET Core SDK](https://www.microsoft.com/net/download) 6.0 or higher
* [Node 16](https://nodejs.org/en/download/)

## Create the database

If you have Docker installed, run the following in your shell

``` sh
docker run --name safe-hangfire -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```

Then use your method of choice to create a database in that database server called SafeHangfire. For example, connect to the database server using Azure Data Studio and execute the following query

``` tsql
CREATE DATABASE [SafeHangfire]
GO
```

If you create your database in a different manner, you will need to update the connection string in ./src/Server/appsettings.json.

## Starting the application

Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

``` bash
dotnet tool restore
```

To concurrently run the server and the client components in watch mode use the following command:

``` bash
dotnet run
```

The app is available at `http://localhost:8080`. You can use the "Queue a job" button to send a request to the server to queue a job. Check the server logs after queuing the job to see the effect.

You can see the status of jobs in the Hangfire dashboard, available at `http://localhost:8085/hangfire`. You can also check the database.
