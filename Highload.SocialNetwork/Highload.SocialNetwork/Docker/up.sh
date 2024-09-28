# Дадим права на запуск скриптов, права тоже копируются в контейнер
chmod +x 10_update_conf.sh

# запустим мастер постгри
docker-compose -f docker-compose.db-master.yaml up -d
docker exec pgmaster mkdir /pgslave

# ждем, когда будет ready
echo "Waiting for master to start"
for i in `seq 1 200`; do \
 docker exec pgmaster pg_isready --host localhost --port 5432 > /dev/null 2> /dev/null && echo "complete in $i sec" && break || sleep 1;
done

docker exec pgmaster pg_isready --host localhost --port 5432 > /dev/null 2> /dev/null || echo "Постгрес не поднялся"

# создадим бекап
echo 'select pg_reload_conf();'| docker exec -i pgmaster su - postgres -c psql
echo "pass" | docker exec -i pgmaster pg_basebackup -h pgmaster -D /pgslave -U replicator -v -P --wal-method=stream

for number in 1 2
do
    # Очистим данные реплики
    rm -rf volumes/replica$number-data
    docker cp pgmaster:/pgslave volumes/replica$number-data/
    touch volumes/replica$number-data/standby.signal
    # восстановим postgres.conf на реплике
    rm -rf volumes/replica$number-data/postgresql.conf
    cp volumes/replica$number-data/postgresql.conf.orig volumes/replica$number-data/postgresql.conf

    # дополним конфиг реплики
    echo "listen_addresses = '*'" >> volumes/replica$number-data/postgresql.conf
    echo "hot_standby = on" >> volumes/replica$number-data/postgresql.conf
    echo "primary_conninfo = 'host=pgmaster port=5432 user=replicator password=pass application_name=pgreplica$number'" >> volumes/replica$number-data/postgresql.conf
done

# Запустим реплики
docker-compose -f docker-compose.db-master.yaml -f docker-compose.db-replica.yaml up -d

# ждем, когда будет ready
for number in 1 2
do
    echo "Waiting for replica$number to start"
    for i in `seq 1 120`; do \
        docker exec pgreplica$number pg_isready --host localhost --port 5432 > /dev/null 2> /dev/null && echo "complete in $i sec" && break || sleep 1;
    done
done

# Проверим статус реплик
echo 'select application_name, sync_state from pg_stat_replication;'| docker exec -i pgmaster su - postgres -c psql

docker-compose -f docker-compose.db-master.yaml -f docker-compose.db-replica.yaml -f docker-compose.api.yaml up -d --no-deps --build highload-api
#docker-compose -f docker-compose.api.yaml up -d
