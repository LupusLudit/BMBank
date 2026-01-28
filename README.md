# BMBank – P2P Node & UI Client

> **Note:** This is a school project. <br>

`Authors: Pavel Halík, Michal Bielina`

This project implements a distributed P2P banking system, consisting of:

- **Bank Node (TCP Server)** – handles banking logic, accounts, commands, and P2P communication
- **UI Client (WPF)** – graphical monitoring and reporting application connected to the bank node and database

Each bank runs as an independent node identified by its **IPv4 address**, communicates with other banks over TCP/IP, and stores data in a **Microsoft SQL Server database**.

The latest BMBank release: [v.1.0.1](https://github.com/LupusLudit/BMBank/releases/tag/v.1.0.1).

## Technologies Used

- .NET 8
- C#
- WPF (MVVM)
- TCP/IP sockets
- Microsoft SQL Server
- XML documentation


## Key Features

### Bank Node
- TCP server for handling client commands
- Account creation and management
- Deposits, withdrawals, balance checks
- Persistent storage using Microsoft SQL Server
- Command logging with automatic cleanup
- P2P communication with other bank nodes
- Robbery plan calculation across the network
- Thread-safe logging and safe execution utilities

### UI Application
- WPF application using MVVM pattern
- Live monitoring of:
  - Active accounts
  - Closed accounts
  - Recent client commands
- Periodic automatic data refresh
- TCP-based server availability checks
- Read-only access to database views

## Installation & Setup

### 1. Database Setup

1. Open **SQL Server Management Studio**
2. Open the file Server/Sql/setup.sql located in the project repository and execute it.

The database is required for **both Node and UI**.

---

### 2. Application Configuration

#### Server config

Locate `App.config` (`Server.dll.config` in the release) in the root directory.

```xml
<appSettings>
  <!--Database config-->
  <add key="DataSource" value="YOUR_SQL_SERVER" />
  <add key="Database" value="YOUR_DATABASE" />
  <add key="Login" value="YOUR_USERNAME" />
  <add key="Password" value="YOUR_PASSWORD" />

  <!--Network scanner config-->
  <add key="BankPortFrom" value="STARTING_PORT"/>
  <add key="BankPortTo" value="ENDING_PORT"/>
  <add key="BankNetworkPrefix" value="NETWORK_PREFIX"/>
</appSettings>
```

- DataSource: Your SQL Server instance address.
- Database: The name of the database.
- Login: Your database username.
- Password: Your database password.
- BankPortFrom: Starting TCP port number used when scanning the network for other bank nodes
- BankPortTo: Ending TCP port number used when scanning the network for other bank nodes
- BankNetworkPrefix: IPv4 network prefix used for discovering other bank nodes in the P2P network

---

#### UI config

Locate `App.config` (`UI.dll.config` in the release) in the root directory.

```xml
<appSettings>
  <!--Main server config-->
  <add key="ServerIp" value="SERVER_IP">
  <add key="ServerPort" value="SERVER_PORT">

  <!--Database config-->
  <add key="DataSource" value="YOUR_SQL_SERVER" />
  <add key="Database" value="YOUR_DATABASE" />
  <add key="Login" value="YOUR_USERNAME" />
  <add key="Password" value="YOUR_PASSWORD" />
</appSettings>
```
- ServerIp: IPv4 address of the bank server the UI connects to via TCP
- ServerPort: TCP port on which the bank server is listening
- DataSource: Your SQL Server instance address.
- Database: The name of the database.
- Login: Your database username.
- Password: Your database password.

---

### 3. Running the Applications

1. Start Bank Node using **Server.exe**

    - This launches the TCP server

    - The node announces itself using its local IPv4 address

2. Start UI Application using **UI.exe**

    - Connects to the same database

    - Communicates with the node via TCP

    - Displays live monitoring data

## Supported Commands

| Name                     | Code | Call                          | Success Response        |
|--------------------------|------|-------------------------------|-------------------------|
| Bank code                | BC   | `BC`                          | `BC <IP>`               |
| Account create           | AC   | `AC`                          | `AC <ACCOUNT>/<IP>`     |
| Account deposit          | AD   | `AD <ACCOUNT>/<IP> <AMOUNT>`  | `AD`                    |
| Account withdrawal       | AW   | `AW <ACCOUNT>/<IP> <AMOUNT>`  | `AW`                    |
| Account balance          | AB   | `AB <ACCOUNT>/<IP>`           | `AB <BALANCE>`          |
| Account remove           | AR   | `AR <ACCOUNT>/<IP>`           | `AR`                    |
| Bank total amount        | BA   | `BA`                          | `BA <AMOUNT>`           |
| Bank number of clients   | BN   | `BN`                          | `BN <COUNT>`            |
| Robbery plan             | RP   | `RP <AMOUNT>`                 | `RP <MESSAGE>`          |

**Robbery plan** - extension to calculate plan to rob other banks in the same network for selected amount. 

## System Workflow

1. Client connects to bank node via TCP
2. Command is received and validated
3. Command is executed locally or forwarded to another bank
4. Result is returned to the client
5. Command execution is logged
6. UI periodically reads updated data from DB and server

## UI – How It Is Connected

- TCP services
    - Check if server is running
    - Retrieve runtime information

- Database services
    - Read-only access to:
        - Active accounts
        - Closed accounts
        - Recent client commands

- ViewModels
    - Expose observable collections
    - Handle refresh timers
    - Coordinate TCP + DB access

UI never modifies data directly – all changes go through the bank node.

## Reused past projects

**Pavel Halík**
- <a href="https://github.com/Forkxel/Quiz-game">Configuration File</a>
- <a href="https://github.com/Forkxel/Chat-Server">TCP connection</a>
- <a href="https://github.com/Forkxel/Library-Database-Manager">Database manipulation</a>

**Michal Bielina**

