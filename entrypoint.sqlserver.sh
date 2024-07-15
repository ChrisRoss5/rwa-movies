#!/bin/bash

# Start the script in background
./configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr