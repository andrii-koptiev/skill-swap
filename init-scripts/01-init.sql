-- Initial database setup script for SkillSwap
-- This script runs automatically when PostgreSQL container starts for the first time

-- Create extensions that might be useful
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "citext";

-- Set timezone
SET timezone = 'UTC';

-- You can add initial table creation, seed data, or other setup here