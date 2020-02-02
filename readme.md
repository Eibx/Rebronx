![Rebronx](https://rebronx.com/rebronx.png)
> A Persistent Browser-based game

Rebronx is a game side-project I work on from time to time.
It's a cyber-themed game based around organized crime.

## Prerequisites

In order to get things running you will need the following.

- [.NET Core](https://www.microsoft.com/net/download/)
- [Node.js](https://nodejs.org/en/)
- [PostgreSQL](https://www.postgresql.org/download/)

## Developing

First clone the project using Git:

```shell
git clone https://gitlab.com/eibx/rebronx.git
cd rebronx
```

Directory name | Description
-------------- | --------------
server | The WebSocket server using .NET Core (backend)
client | Vue.js site (fronend)
data | Data shared between the frontend and backend, such as item and map information


Then create a self-signed certificate:

```shell
openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost/O=Rebronx/OU=Rebronx'
openssl pkcs12 -export -inkey key.pem -in cert.pem -out rebronx.p12 -password pass:1
```

*Note that browsers will still not trust these certificates.*
*So after we get the server and site running - make exceptions for https://localhost:8080 and https://localhost:21220.*

Go to `server/`

Create config file based on sample

```shell
cp app.config.sample app.config
```

Build server project

```shell
dotnet build
```

Set up of PostgreSQL

```shell
sudo -i -u postgres psql -c "CREATE USER rebronx;"
sudo -i -u postgres psql -c "CREATE DATABASE rebronx;"
sudo -i -u postgres psql -c "GRANT ALL PRIVILEGES ON DATABASE rebronx to rebronx;"
```

Go to `server/migration/` and run:

```shell
psql -h localhost rebronx rebronx -f 000-init.sql
```

Go to `client/` and run:

```shell
npm install
npm run serve
```

## Licensing

The code in this project is licensed under MIT license.