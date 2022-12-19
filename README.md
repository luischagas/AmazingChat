# Amazing Chat

To execute the project, the following step is necessary: 

## Installation

- Make sure the Docker is running and ready on your machine 

- Run the shell script with the name .RUN (located at the root of the repository), it will create the required containers:

  - Database SQL 
  - RabbitMQ
  - UI (MVC Application)
  - Stock Bot (Web API Application)

- Then a web browser will open with the address: http://localhost:8080 ready to use Amazing Chat. 

- The tests will be run automatically by the same script that will create the containers, but it is also possible to run them with the command:
  - (It's necessary navigate the folder until you reach the project folder)

```bash
dotnet test AmazingChat.Tests
``` 

## Debug Installation

- Make sure the Docker is running and ready on your machine 
- Run the following docker commands to start RabbitMQ and SQL Server:

```bash
docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1433:1433 -d --name=sql mcr.microsoft.com/azure-sql-edge
``` 

```bash
docker run -d -p 15672:15672 -p 5672:5672 -p 5671:5671 --hostname my-rabbitmq --name my-rabbitmq-container -e RABBITMQ_DEFAULT_USER=rabbitmq -e RABBITMQ_DEFAULT_PASS=PASSWORD rabbitmq:3-management-alpine
``` 

- Run the followings commands to create both databases by entity framework migrations:

```bash
update-database -context AmazingChatIdentityContext
``` 

```bash
update-database -context AmazingChatContext
``` 

- Then it's necessary set multiple startup projects:
  - AmazingChat.UI
  - AmazingChat.StockBot

## What was implemented? 

- Clean architecture 
- .NET 6
- Authentication with Identity
- Entity Framework Core 6
- Unit Tests using Xunit and Fixture libraries
- Docker Support
- RabbitMQ

## What features were implemented? 

- Chatroom 
- Multiples room can be created
- Handle messages that are not understood or any exceptions 
  - (It was implemented a exception middleware on Web API and it was implemented validations on MVC Application)
- Installer
- Messages ordered by their Timestamps
- Command to get stocks prices /stock=stock_code and validation of invalid commands
- Limit of 50 messages per chat
- Messages being save into Sql Database so a new user could read previous messages
