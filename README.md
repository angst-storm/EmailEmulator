## Запуск:

### Docker:

```shell
docker volume create redpanda-data
docker-compose up -d
```

### Redpanda:

```shell
rpk topic create mails
rpk topic create clicks
```