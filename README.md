# Сервис публикации блек-метала из ВК в Телеграм

Канал Телеграм, в который это публикуется `на чёрный день` (https://t.me/for_black_day)

Публикация не универсальна. Публикуются только плейлисты из выбранных автором пабликов ВК. Поскольку нет возможности в
самом api ВК получать нужную информацию, то приходится парсить тексты публикаций. И поскольку это разные паблики разных
авторов, то у них нет единства стиля, поэтому парсить можно не всё. Поэтому добавлены только следующие паблики:

- `Blackwall` (https://vk.com/blackwall)
- `Убежище Лесников` (https://vk.com/asylumforesters_vk)
- `Black Metal Promotion` (https://vk.com/black_metal_promotion)
- `E:\music\black metal` (https://vk.com/e_black_metal)
- `E:\music\post` (https://vk.com/post_music)
- `E:\music\progressive metal` (https://vk.com/progressivemetal)
- `Русский блэк-метал` (https://vk.com/ru_black_metal)

## Билд и развёртывание на сервере

Переменные окружения

- `VK_TOKEN` — токен ТГ
- `TG_BOT_ID` — токен ВК
- `TG_CHANNEL_ID` — канал в ТГ
- `JOB_CRON_LAST` — крон выражение для сбора последних публикаций (новинок)
- `JOB_CRON_TOP_WEEK` — крон выражение для сбора топ публикаций за неделю
- `JOB_CRON_TOP_MONTH` — крон выражение для сбора топ публикаций за месяц
- `VK_LAST_COUNT` — количество новинок
- `TG_TOP_COUNT` — количество топ публикаций

```shell
# build
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained false

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
