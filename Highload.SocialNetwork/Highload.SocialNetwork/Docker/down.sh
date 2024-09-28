
docker-compose -f docker-compose.db-master.yaml -f docker-compose.db-replica.yaml -f docker-compose.api.yaml down
rm -rf volumes/master-data

for number in 1 2
do
    # Очистим данные реплики
    rm -rf volumes/replica$number-data
done