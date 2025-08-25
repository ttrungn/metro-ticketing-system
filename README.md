# MetroTicketingSystem

MetroTicketingSystem is a modular, microservices-based solution for managing metro ticketing operations. It leverages Clean Architecture principles for maintainability and scalability. The system is composed of several services, including CatalogService, NotificationService, OrderService, SampleService, UserService, and YarpApiGateway, each responsible for a specific domain within the ticketing workflow.

## Contributors

| Name               | Role       |
|--------------------|------------|
| Nguyen Thanh Trung | Leader     |
| Bui Gia Huy        | Developer  |
| Le Hoang Khanh     | Developer  |
| Nguyen Tri Quyet   | Developer  |

## Related Projects

Here are some related repositories for the metro ticketing system:

- [Mobile Application (Flutter/Dart)](https://github.com/ttrungn/metro-ticketing-system-mobile)  
  A mobile app for passengers to book, manage, and validate metro tickets.

- [Web Administration Panel (React/Node.js)](https://github.com/SANG-VAN-PHAN/metro_ticket_system_admin)  
  A web-based admin dashboard for managing tickets, routes, and system data.

Each service in the project project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/SampleService) version 8.0.6.

## Build

Run `dotnet build -tl` to build the solution.

## Run

To run the web application:

```bash
cd .\src\Web\
dotnet watch run
```

Navigate to https://localhost:5001. The application will automatically reload if you change any of the source files.

## Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The **.editorconfig** file defines the coding styles applicable to this solution.

## Code Scaffolding

The template includes support to scaffold new commands and queries.

Start in the `.\src\Application\` folder.

Create a new command:

```
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

Create a new query:

```
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

If you encounter the error *"No templates or subcommands found matching: 'ca-usecase'."*, install the template and try again:

```bash
dotnet new install Clean.Architecture.Solution.Template::8.0.6
```

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

## Help
To learn more about the template go to the [project website](https://github.com/jasontaylordev/CleanArchitecture). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.
