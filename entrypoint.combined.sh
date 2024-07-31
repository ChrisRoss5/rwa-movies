#!/bin/bash

./configure-db.sh &

/opt/mssql/bin/sqlservr -c &

dotnet RwaMovies.dll
