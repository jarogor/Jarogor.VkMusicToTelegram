```sh
# создание сервисного пользователя, если потребуется
sudo useradd --system --no-create-home --shell=/sbin/nologin vktotg

# 1. создание симлинка сервиса (не уверен в работоспособности, тогда вместо симлинка создать файл сервиса)
ln -s /opt/vktotg/vktotg.service /etc/systemd/system/
# 2. перечитывание настроек сервисов systemd
sudo systemctl daemon-reload
# 3. добавление автозапуск, чтобы при перезагрузке стартовало само
sudo systemctl enable vktotg.service

# запуск
sudo systemctl start vktotg.service
# проверка статуса
sudo systemctl status vktotg.service

# остановка
sudo systemctl stop vktotg.service
```
