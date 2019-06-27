FROM mono:latest as builder

COPY . /windbot-source
WORKDIR /windbot-source
RUN xbuild /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:OutDir=/windbot/

RUN wget -O /windbot/cards.cdb https://github.com/purerosefallen/ygopro-database/raw/master/locales/zh-CN/cards.cdb

FROM mono:silm
COPY --from=builder /windbot /

WORKDIR /windbot

EXPOSE 2399
CMD [ "mono", "/windbot/WindBot.exe", "ServerMode=true", "ServerPort=2399" ]
