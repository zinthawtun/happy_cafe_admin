# Production Deployment

This directory contains files for deploying the Happy Cafe Admin backend in a production environment using Docker.

## Prerequisites

- Docker and Docker Compose installed
- Access to the project repository

## Configuration

1. Edit the `.env` file to set your production database credentials
   - Replace `prod_user` with your production database username
   - Replace `secure_password_here` with a strong password
   - Update other variables as needed

## Deployment Steps

1. Navigate to the setup_prod directory:
   ```bash
   cd setup_prod
   ```

2. Build and start the containers:
   ```bash
   docker-compose up -d
   ```

3. To check logs:
   ```bash
   docker-compose logs -f
   ```

4. To stop the deployment:
   ```bash
   docker-compose down
   ```

## Exposed Services

- Backend API: http://localhost:8080 (HTTP), https://localhost:8443 (HTTPS)
- PostgreSQL: localhost:5432 (accessible via connection string in the backend)

## Important Notes

- Make sure to use secure passwords in the production environment
- Consider setting up SSL for production deployments
- For deployment to remote servers, you might need to adjust network settings 