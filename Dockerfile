FROM mono:latest as builder

RUN apt update && env DEBIAN_FRONTEND=noninteractive apt install -y wget

COPY . /windbot-source
WORKDIR /windbot-source
RUN xbuild /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:OutDir=/windbot/

RUN wget -O /windbot/cards.cdb https://github.com/purerosefallen/ygopro-database/raw/master/locales/zh-CN/cards.cdb

FROM mono:silm
COPY --from=builder /windbot /

WORKDIR /windbot

EXPOSE 2399
CMD [ "mono", "/windbot/WindBot.exe", "ServerMode=true", "ServerPort=2399" ]
