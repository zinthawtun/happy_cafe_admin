# Happy Cafe Admin

A full-stack application for managing cafe administration.

## Project Structure

- `frontend/`: React application with TypeScript
- `backend/`: .NET API application

## Development Setup

### Prerequisites

- Node.js v22.14.0
- .NET 9.0.201 SDK
- npm
- Docker 

### Preparation

#### Make sure docker is running before we do the following steps:

1. Go to \setup_dev from the project <root> directory
2. Create .env files with the following format. 
```bash 
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_USER=<username>
DB_PASSWORD=<password>
DB_NAME=<db_name>

# File Storage Configuration
FILE_STORAGE_ROOT_PATH=
FILE_STORAGE_PATH=FileStore
FILE_STORAGE_LOGOS_PATH=FileStore/logos
FILE_STORAGE_MAX_SIZE=2097152
```
3. Run the docker-compose to create DB in Postgres. 
```bash
docker-compose up -d
```

4. Run the following to add migrations and seed the data.
```bash
dotnet ef migrations add SeedData
dotnet ef database update
```

### Running the Application

#### Option 1: Run Both Frontend and Backend Simultaneously

From the root directory, run:

```bash
npm run dev
```

This will start both the frontend and backend applications in parallel using concurrently.

#### Option 2: Run Frontend and Backend Separately

To run only the frontend:

```bash
npm run frontend
```

To run only the backend:

```bash
npm run backend
```

To start application installing node_modules

#### Option 3: Run Both Frontend and Backend Simultaneously after installing node_modules

```bash
npm run start
```

## Additional Information

- Frontend runs on: http://localhost:5173
- Backend API runs on:  http://localhost:5222