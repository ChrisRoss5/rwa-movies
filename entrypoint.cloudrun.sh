#!/bin/bash

#mkdir -p /var/opt/mssql/data
#chown -R mssql:mssql /var/opt/mssql/data
#chmod -R 700 /var/opt/mssql/data

# Start the script in background
./configure-db.sh &

/opt/mssql/bin/sqlservr &

dotnet RwaMovies.dll
