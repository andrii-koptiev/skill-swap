# Docker Setup for SkillSwap API

## PostgreSQL Database Setup

This directory contains the Docker Compose configuration for running PostgreSQL locally for development.

### Quick Start

1. **Start PostgreSQL:**

   ```bash
   docker-compose up -d
   ```

2. **Stop services:**

   ```bash
   docker-compose down
   ```

3. **Stop and remove volumes (⚠️ This will delete all data):**
   ```bash
   docker-compose down -v
   ```

### Database Connection Details

- **Host:** localhost
- **Port:** 5432
- **Database:** skillswap
- **Username:** skillswap_user
- **Password:** skillswap_password

### Connection String

The connection string is already configured in `appsettings.Development.json`:

```
Host=localhost;Port=5432;Database=skillswap;Username=skillswap_user;Password=skillswap_password;Include Error Detail=true
```

### Database Initialization

- Initial setup scripts are located in `init-scripts/`
- These scripts run automatically when the database container starts for the first time
- Add your schema creation and seed data scripts here

### Health Checks

The PostgreSQL container includes health checks to ensure the database is ready before your application connects.

### Data Persistence

Database data is persisted in Docker volumes:

- `postgres_data`: PostgreSQL data

### Network

All services run on the `skillswap-network` for easy inter-service communication.
