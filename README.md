## Запуск:

### Docker:

```shell
docker build -t net-sender -f Sender/Dockerfile .
docker build -t net-reader -f Reader/Dockerfile .
docker build -t net-analyzer -f Analyzer/Dockerfile .
docker volume create redpanda-data
docker-compose up -d
```

### Redpanda:

```shell
rpk topic create mails
rpk topic create clicks
```