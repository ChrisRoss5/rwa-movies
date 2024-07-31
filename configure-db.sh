#!/bin/bash

echo "[configure-db.sh] Waiting for SQL Server to start up on $SERVER_NAME"

# To target specific SQL Server instance, use the -S option like so:
# /opt/mssql-tools/bin/sqlcmd -S $SERVER_NAME

start_time=$(date +%s%3N)
DBSTATUS=1
ERRCODE=0
i=0

while ([[ $DBSTATUS -ne 0 ]] || [[ $ERRCODE -ne 0 ]]) && [[ $i -lt $INIT_TIMEOUT ]]; do
  i=$((i + 1))
  DBSTATUS=$(/opt/mssql-tools/bin/sqlcmd -h -1 -t 1 -U sa -P $SA_PASSWORD -Q "SET NOCOUNT ON; SELECT SUM(state) FROM sys.databases")
  ERRCODE=$?

  echo "[configure-db.sh] DEBUG: Iteration=$i"
  echo "[configure-db.sh] DEBUG: DBSTATUS=$DBSTATUS"
  echo "[configure-db.sh] DEBUG: ERRCODE=$ERRCODE"
  sleep 1
done

if [[ $DBSTATUS -ne 0 ]] || [[ $ERRCODE -ne 0 ]]; then
  echo "[configure-db.sh] SQL Server took more than $INIT_TIMEOUT tries to start up or one or more databases are not in an ONLINE state"
  exit 1
fi

echo "[configure-db.sh] SQL Server started"

/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -d master -i setup.sql
echo "[configure-db.sh] SQL Server setup completed"

/opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -d master -i seed.sql
echo "[configure-db.sh] SQL Server seed completed"

end_time=$(date +%s%3N)
duration_ms=$((end_time - start_time))
echo "[configure-db.sh] Total time in ms: $duration_ms"
