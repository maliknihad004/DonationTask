
# Donation Management System

A REST API built using ASP.NET Core (.NET 10), C#, Entity Framework Core, and PostgreSQL.

The system allows users to create, retrieve, update, and delete donations. It also supports anonymous donations, input validation, and pagination.

This project is backend-only. No frontend or authentication is required.

## 1. Technologies Used

- C#
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- PostgreSQL
- Docker and Docker Compose
- Swagger (if enabled in the application)
- Postman

## 2. Project Structure

```text
DonationTask/
├── Controllers/
│   ├── DonationsController.cs
│   └── HealthController.cs
├── Data/
│   └── DonationDbContext.cs
├── DTOs/
│   ├── DonationRequest.cs
│   └── DonationResponse.cs
├── Models/
│   └── Donation.cs
├── Migrations/
├── postman/
├── Program.cs
├── DonationTask.csproj
├── Dockerfile
├── compose.yaml
├── .dockerignore
├── .env
└── README.md
```

## 3. Setup (Without Docker)

### Requirements

To run the project directly on your computer, install:

- .NET 10 SDK
- PostgreSQL
- Entity Framework Core CLI
- Postman (for testing)

Install the EF Core CLI if it is not already installed:

```bash
dotnet tool install --global dotnet-ef
```

Open the project directory:

```bash
cd ~/internship/DonationTask
```

Restore the required packages:

```bash
dotnet restore
```

**Note:** If you use Docker Compose instead, you only need Docker with Docker Compose. You do not need to install .NET or PostgreSQL locally.

## 4. Database Configuration (Without Docker)

The application uses PostgreSQL to store donation records.

### Start PostgreSQL

```bash
sudo service postgresql start
```

### Create the Database

Open PostgreSQL:

```bash
sudo -u postgres psql
```

Create the database:

```sql
CREATE DATABASE donation_management;
```

Exit PostgreSQL:

```sql
\q
```

### Configure the Connection String

Create a `.env` file in the project root directory.

Add the following connection string for running without Docker:

```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=donation_management;Username=postgres;Password=YOUR_PASSWORD
```

Replace `YOUR_PASSWORD` with your PostgreSQL password.

The application loads the environment configuration when it starts.

Do not upload the `.env` file to GitHub because it contains database credentials. Add `.env` to `.gitignore` and `.dockerignore`.

### Apply Database Migrations

Run:

```bash
dotnet ef database update
```

This applies the existing EF Core migrations and creates the required database table. The application is also configured to apply pending migrations on startup.

### Database Design

The system uses one main table called `Donations`.

| Column | Description |
|---|---|
| Id | Primary key |
| Amount | Donation amount |
| DonorName | Name of the donor |
| IsAnonymous | Indicates whether the donation is anonymous |
| CreatedAt | Date and time of donation creation |

### Database Normalization

The database follows the basic normalization rules:

- **1NF:** Each column contains a single atomic value.
- **2NF:** All non-key attributes depend on the primary key.
- **3NF:** There are no transitive dependencies between non-key attributes.

Since the current system manages only donations, one table is sufficient.

## 5. Running the Application (Without Docker)

Build the project:

```bash
dotnet build
```

Run the API:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --urls http://127.0.0.1:5010
```

The application will run at:

http://127.0.0.1:5010

### Health Check

To verify that the API is running, open:

http://127.0.0.1:5010/api/health

Expected response:

```json
{
  "status": "Healthy"
}
```

HTTP Status: `200 OK`

## 6. Running with Docker Compose

Docker Compose runs the ASP.NET Core API and PostgreSQL database in separate containers.

### Requirements

Install Docker and Docker Compose.

You do not need to install .NET or PostgreSQL on your computer.

### Step 1: Configure the Password

Create a `.env` file in the same folder as `compose.yaml`.

Add:

```env
DB_PASSWORD=YOUR_PASSWORD
```

Replace `YOUR_PASSWORD` with your chosen password.

The `compose.yaml` file uses this password for PostgreSQL and gives the API this connection string:

```text
Host=db;Port=5432;Database=donation_management;Username=postgres;Password=${DB_PASSWORD}
```

`db` is the PostgreSQL service name inside Docker, so the API does not use `localhost` to connect to the database.

Docker Compose automatically creates a shared bridge network for both containers.

### Step 2: Build and Start

Open the terminal in the project directory:

```bash
cd ~/internship/DonationTask
```

Build and start the containers:

```bash
docker compose up --build -d
```

This command:

- Builds the API Docker image.
- Starts the PostgreSQL container.
- Waits until PostgreSQL is healthy.
- Starts the API container.
- Applies the existing database migrations when the API starts.

### Step 3: Check the Containers

Run:

```bash
docker compose ps
```

Both containers should be running, and PostgreSQL should show as healthy.

To see the API logs:

```bash
docker compose logs api
```

The API should display:

```text
Now listening on: http://[::]:8080
Application started.
```

### Step 4: Test the API

Open Postman.

Select the GET method and enter:

```http
GET http://localhost:8081/api/Donations
```

Click **Send**.

A successful request returns `200 OK`.

Example response when the database is empty:

```json
{
  "totalDonations": 0,
  "totalPages": 0,
  "currentPage": 1,
  "pageSize": 1,
  "donations": []
}
```

The API runs on port `8080` inside Docker, which is mapped to port `8081` on the computer.

You can also test the health endpoint:

```http
GET http://localhost:8081/api/health
```

### Step 5: Stop the Containers

Run:

```bash
docker compose down
```

To start the containers again:

```bash
docker compose up -d
```

### Docker Database Connection

The API and PostgreSQL containers communicate through the Docker Compose network.

The API uses `Host=db` to connect to the database container.

A PostgreSQL health check ensures that the API waits until the database is ready before starting.

Entity Framework Core applies the existing migrations to create the required tables.

**Note:** The Docker database is separate from the PostgreSQL database installed on your computer. A named Docker volume should be used if donation data must survive container removal and recreation.

## 7. API Endpoints

The application supports the following REST API endpoints.

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/health | Check API health |
| GET | /api/donations | Retrieve donations with pagination |
| GET | /api/donations/{id} | Retrieve a donation by ID |
| POST | /api/donations | Create a new donation |
| PUT | /api/donations/{id} | Update an existing donation |
| DELETE | /api/donations/{id} | Delete a donation |

### HTTP Status Codes

| Status Code | Meaning |
|---|---|
| 200 OK | Request completed successfully |
| 201 Created | Donation created successfully |
| 204 No Content | Donation deleted successfully |
| 400 Bad Request | Invalid request data |
| 404 Not Found | Donation does not exist |

### Example: Create Donation

**POST /api/donations**

Request body:

```json
{
  "amount": 100,
  "donorName": "Ahmad",
  "isAnonymous": false
}
```

Expected Status: `201 Created`

### Example: Anonymous Donation

```json
{
  "amount": 200,
  "donorName": "Sara",
  "isAnonymous": true
}
```

When a donation is anonymous, the API hides the original donor name.

The response returns:

```json
{
  "donorName": "Anonymous",
  "isAnonymous": true
}
```

The real donor name is stored in the database but is never included in donation API responses when `IsAnonymous` is true.

### Pagination

The API supports pagination when retrieving donations.

Example:

```http
GET /api/donations?page=1
```

The response includes:

- `totalDonations`
- `totalPages`
- `currentPage`
- `pageSize`
- `donations`

The application automatically calculates the page size based on the number of donations, with a maximum of 200 records per page.

Invalid page numbers return `400 Bad Request`.

## 8. Swagger Access

Swagger provides an interactive interface for testing API endpoints when it is configured and enabled in the application.

### Steps

1. Start the application in the Development environment with Swagger enabled.
2. Open a browser.
3. Navigate to http://127.0.0.1:5010/swagger.
4. Select an API endpoint.
5. Click **Try it out**.
6. Enter the required parameters or JSON body.
7. Click **Execute**.

Swagger displays the request, response, and HTTP status code.

The Docker setup runs in Production mode, so use Postman for Docker API testing unless Swagger is separately enabled.

## 9. Postman Testing

Postman was used to test the REST API and verify the expected responses.

The exported Postman collection is included in the `postman` directory.

### Import the Collection

1. Open Postman.
2. Click **Import**.
3. Select the exported Postman collection JSON file.
4. Import the collection.
5. Start the API.
6. Open a request and click **Send**.
7. Check the response and automated test results.

For Docker, use `http://localhost:8081` as the base URL.

For the local setup, use `http://127.0.0.1:5010`.

### Tests Performed

The following scenarios were tested:

| Test | Expected Status |
|---|---|
| Health Check | 200 |
| Create Donation | 201 |
| Create Anonymous Donation | 201 |
| Get All Donations | 200 |
| Get Donation by ID | 200 |
| Update Donation | 200 |
| Update Anonymous Donation | 200 |
| Delete Donation | 204 |
| Verify Deleted Donation | 404 |
| Negative Donation Amount | 400 |
| Zero Donation Amount | 400 |
| Missing Donation Amount | 400 |
| Missing Donor Name | 400 |
| Missing IsAnonymous | 400 |
| Invalid Donation ID | 400 |
| Get Non-Existent Donation | 404 |
| Update Non-Existent Donation | 404 |
| Delete Non-Existent Donation | 404 |
| Invalid Page Number | 400 |

Postman automated test scripts use `pm.test()` to verify HTTP status codes and response values.

Some requests depend on donations created by previous requests. These requests should be executed in the correct order when testing the collection.

## 10. Key Design Decisions

### REST API Architecture

The system uses ASP.NET Core Controllers to implement CRUD operations through standard HTTP methods.

Each endpoint returns an appropriate HTTP status code.

### Entity Framework Core

Entity Framework Core is used to interact with PostgreSQL.

EF Core migrations are used to create and manage the database schema.

### Data Transfer Objects (DTOs)

Separate DTO classes are used:

- `DonationRequest`: Defines the information accepted from clients.
- `DonationResponse`: Defines the information returned by the API.

This separates the database entity from the public API response.

### Anonymous Donor Privacy

The application stores the donor's original name in the database.

However, when `IsAnonymous` is true, the API returns `"Anonymous"` instead of the real name.

This rule is applied to donation responses when creating, retrieving, or updating donations.

### Input Validation

The application validates donation requests before saving data.

Validation rules include:

- Donation amount must be greater than zero.
- Donation amount is required.
- Donor name is required.
- IsAnonymous is required.
- Donation IDs must be valid.
- Non-existent donations return `404 Not Found`.

Invalid input returns `400 Bad Request`.

### Pagination

Pagination is used to avoid returning all donation records in a single response.

The page size is calculated automatically and is limited to a maximum of 200 records per page.

### UTC Timestamps

The `CreatedAt` field is generated by the server using UTC when a donation is created.
