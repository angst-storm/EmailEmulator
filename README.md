## Запуск:

### Docker:

```shell
docker network create email
docker-compose up -d
```

### Kafka:

```shell
kafka-topics.sh --create --topic commands --bootstrap-server localhost:9092
kafka-topics.sh --create --topic emails --bootstrap-server localhost:9092
```

## Использование:

```shell
kafka-console-producer.sh --topic commands --bootstrap-server localhost:9092
>user username
>message messagetext
^C
kafka-console-consumer.sh --topic emails --from-beginning --bootstrap-server localhost:9092
```