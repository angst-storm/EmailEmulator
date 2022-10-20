## Запуск реализации на .NET:

### Docker:

```shell
docker volume create redpanda-data
docker-compose up -d
```

### Redpanda:

```shell
rpk topic create commands
rpk topic create emails
```

## Использование:

```shell
rpk topic produce commands
>user username
>message messagetext
^C
rpk topic consume emails
```