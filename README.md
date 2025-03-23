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

## Additional Information

- Frontend runs on: http://localhost:5173
- Backend API runs on:  http://localhost:5222