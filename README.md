## Запуск:

### Docker:

```shell
docker build -t net-sender -f Sender/Dockerfile .
docker build -t net-reader -f Reader/Dockerfile .
docker build -t net-estimator -f Estimator/Dockerfile .
docker build -t metabase -f MetaBase/Dockerfile .
docker volume create redpanda-data
docker volume create postgres-data
docker-compose up -d
```

### Redpanda:

```shell
rpk topic create mails
rpk topic create clicks
rpk topic create commands
rpk topic create errors
```

### ClickHouse

```shell
clickhouse-client
```

```sql
create table clicks (userId UInt64, mailThemes Array(UInt64)) engine = Kafka settings kafka_broker_list = 'redpanda:9092', kafka_topic_list = 'clicks', kafka_group_name = 'clickhouse', kafka_format = 'JSONEachRow';
create table clicks_log (userId UInt64, mailThemes Array(UInt64)) engine = Log;
create materialized view clicks_log_view to clicks_log as select userId, mailThemes from clicks;
select * from clicks_log;

create table errors (totalClicksCount UInt64, value double) engine = Kafka settings kafka_broker_list = 'redpanda:9092', kafka_topic_list = 'errors', kafka_group_name = 'clickhouse', kafka_format = 'JSONEachRow';
create table errors_log (totalClicksCount UInt64, value double) engine = Log;
create materialized view errors_log_view to errors_log as select totalClicksCount, value from errors;
select * from errors_log;
```