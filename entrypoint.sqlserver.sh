#!/bin/bash

# Cloud Run attempts to solve error:
# error 87(the parameter is incorrect.) occurred while opening file '/var/opt/mssql/data/master.mdf' to obtain configuration information at startup."

# mkdir -p /var/opt/mssql/data
# chown -R mssql /var/opt/mssql/data
# chmod -R 700 /var/opt/mssql/data

# Start the script in background
./configure-db.sh &

# Start SQL Server
/opt/mssql/bin/sqlservr -c