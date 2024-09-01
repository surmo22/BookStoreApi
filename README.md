# BookStoreApi
The BookStore API is a RESTful service designed for managing a bookstore's inventory, allowing users to perform CRUD (Create, Read, Update, Delete) operations on books, authors, and genres. This document serves as a guide to help you understand and interact with the API.

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

## Prerequisites
You will need the following tools installed on your system:

- .NET 8.0 SDK
- A preferred code editor (e.g., Visual Studio, VS Code)
  
## Installing
Follow these steps to get your development environment set up:

1. Clone the repository
```bash
git clone https://github.com/yourusername/BookStoreApi.git
 ```
2. Setup Sql Server
  - Ensure MySQL server is up and running.
  - Update the connection string in `appsettings.json` with your database credentials.
  ```json
  "ConnectionStrings": {
      "DefaultConnection": "server=localhost;database=TodoListDb;user=root;password=yourpassword;"
  }
  ```
3. **Apply EF Core migrations**:

  ```bash
  dotnet ef database update
  ```
    
## Running the Application   
1. Navigate to the project directory and restore dependencies
```bash
cd BookStoreApi
dotnet restore
```
2. Build the project

```
dotnet build
```
3. Run the project
```
dotnet run --project BookStoreApi
```
The API will start up on http://localhost:5000, or a different port specified in your project configurations.

## Q&A
### Was it easy to complete the task using AI? 
Yes.
### How long did task take you to complete?
Approximately 3 hours.
### Was the code ready to run after generation? What did you have to change to make it usable?
Most of the time the code was ready to run after generation, however i expierenced some problems with tests generation.
### Which challenges did you face during completion of the task?
There were no serious challanges.
### Which specific prompts you learned as a good practice to complete the task?
Generate code for CRUD controler for this model: [code]
