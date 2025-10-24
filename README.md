CpiService API
Overview

CpiService is a scalable ASP.NET Core 8 REST API built in C# that fetches and caches Consumer Price Index (CPI) data from the U.S. Bureau of Labor Statistics (BLS) Public API. The API provides CPI values for a given month and year, along with any associated notes. It implements JWT authentication and in-memory caching to optimize performance and prevent exceeding API call limits.

Features

Fetch CPI data for a specific month and year.

Return CPI value and associated notes.

In-memory caching for 1 day to minimize API requests.

JWT-based authentication for secure access.

Dockerized for easy deployment.

Ready for AWS deployment (ECR + ECS/Fargate or Elastic Beanstalk).

Project Structure
CpiService/
│
├── Controllers/
│   ├── CpiController.cs       # Handles CPI requests
│   └── AuthController.cs      # JWT token generation
│
├── Models/
│   ├── CpiRequest.cs          # Request model (year, month)
│   └── CpiResponse.cs         # Response model (CPI value + notes)
│
├── Services/
│   ├── BlsApiService.cs       # Fetches data from BLS API
│   └── CpiCacheService.cs     # Caches CPI data in memory
│
├── Auth/
│   └── JwtAuthenticationManager.cs   # Generates JWT tokens
│
├── Program.cs                 # Configures services, JWT, Swagger
├── appsettings.json
├── Dockerfile
└── README.md

API Endpoints
1. Get JWT Token

POST /api/auth/token

Request Body:

{
  "username": "admin",
  "password": "password"
}


Response:

{
  "token": "<JWT_TOKEN_HERE>"
}


Use this token for authorization in the Authorization header:

Authorization: Bearer <JWT_TOKEN_HERE>

2. Get CPI Data

GET /api/cpi?year={year}&month={month}

Query Parameters:

year (int): e.g., 2023

month (string): e.g., May

Headers:

Authorization: Bearer <JWT_TOKEN_HERE>


Response:

{
  "cpiValue": 321,
  "notes": ""
}


Notes will be empty if no footnotes are available for the selected month.

Caching

CPI data is cached in memory for 1 day.

Prevents unnecessary calls to the BLS API (which has a rate limit of 25 calls/day).

Cache can be replaced with Redis for distributed deployments.

Running the API
Locally
git clone https://github.com/<your-username>/CpiService.git
cd CpiService
dotnet run


Open Swagger: https://localhost:7065/swagger

Get JWT token via /api/auth/token

Call /api/cpi with Authorization: Bearer <token>

Using Docker
docker build -t cpi-service .
docker run -p 7065:80 cpi-service

AWS Deployment

Build Docker image:

docker build -t cpi-service .


Push to AWS ECR:

aws ecr create-repository --repository-name cpi-service
docker tag cpi-service:latest <aws_account_id>.dkr.ecr.<region>.amazonaws.com/cpi-service:latest
docker push <aws_account_id>.dkr.ecr.<region>.amazonaws.com/cpi-service:latest


Deploy using AWS Fargate (ECS) or Elastic Beanstalk.

Technologies Used

ASP.NET Core 8.0

C# 11

JWT Authentication

IMemoryCache

HttpClient for external API calls

Swagger UI

Docker

Notes

Currently uses national CPI dataset (CUUR0000SA0).

JWT secret is hardcoded for demonstration — replace with AWS Secrets Manager or environment variables in production.

API is stateless, so it scales well in cloud deployments.
