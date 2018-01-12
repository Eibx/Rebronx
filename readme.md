![Rebronx](https://rebronx.com/rebronx.png)
> A Persistent Browser-based game

Rebronx is a game side-project I work on from time to time.
It's a cyber-themed game based around organized crime with permadeath.

## Prerequisites

In order to get things running you will need the following.

- [.NET Core](https://www.microsoft.com/net/download/)
- [Node.js](https://nodejs.org/en/)
- [PostgreSQL](https://www.postgresql.org/download/)

## Developing

First clone the project using Git:

```shell
git clone https://gitlab.com/Eibx/Rebronx.git
cd Rebronx
```

Directory name | Description
-------------- | --------------
Rebronx.Server | The WebSocket server using .NET Core (backend)
Rebronx.UI | Vue.js site (fronend)
Rebronx.Data | Data shared between the frontend and backend, such as item and map information


Then create a self-signed certificate:

```shell
openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost/O=Rebronx/OU=Rebronx'
openssl pkcs12 -export -inkey key.pem -in cert.pem -out rebronx.p12 -password pass:rebronx_pass
```

Go to `Rebronx.Server` and run:

```shell
dotnet restore
dotnet build
```

*TODO: Describe how to run SQL scripts against PostgreSQL*

Open a new terminal, and go to `Rebronx.UI` and run:

```shell
npm install
npm run dev
```

**Not needed for now**

```shell
echo "127.0.0.1  rebronx.test" | sudo tee -a /etc/hosts
```

Note that browsers will still not trust these certificates, so make exceptions for https://localhost:8080 and https://localhost:21220.

## Licensing

The code in this project is licensed under MIT license.