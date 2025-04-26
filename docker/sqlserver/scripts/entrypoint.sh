#!/bin/bash

set -e

/opt/mssql/bin/sqlservr &
pid=$!

sleep 30

# ダウンロード AdventureWorks.bak （なければダウンロード）
if [ ! -f /scripts/AdventureWorks.bak ]; then
  echo "Downloading AdventureWorks backup..."
  wget -q -O /scripts/AdventureWorks.bak "https://github.com/Microsoft/sql-server-samples/releases/download/adventureworks/AdventureWorks2022.bak"

  # データベース復元用SQLを作成
  cat <<EOF > /tmp/restore.sql
USE [master];
RESTORE DATABASE [AdventureWorks]
FROM DISK = '/scripts/AdventureWorks.bak'
WITH MOVE 'AdventureWorks2022' TO '/var/opt/mssql/data/AdventureWorks.mdf',
    MOVE 'AdventureWorks2022_log' TO '/var/opt/mssql/data/AdventureWorks_log.ldf',
    REPLACE;
EOF

  # SQLCMDで復元実行
  /opt/mssql-tools18/bin/sqlcmd -No -S localhost -U SA -P P@ssw0rd -i /tmp/restore.sql

  echo "AdventureWorks database restored."

fi

wait $pid
