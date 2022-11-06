## Запуск:

### Docker:

```shell
docker volume create redpanda-data
docker volume create postgres-data
docker-compose up -d
```

### Redpanda:

```shell
rpk topic create mails
rpk topic create clicks
rpk topic create commands
```

### NET:

Запустить в таком порядке: Estimator, Sender, Reader