FROM debian:trixie-slim as resource-loader
ARG NO_RESOURCE=0
RUN mkdir -p /resources/windbot && \
  if [ "$NO_RESOURCE" != "1" ]; then \
    apt update && apt -y install git && \
    git clone --depth=1 https://github.com/purerosefallen/ygopro-database /ygopro-database && \
    cp -f /ygopro-database/locales/zh-CN/cards.cdb /resources/windbot/ && \
    rm -rf /var/lib/apt/lists/* /ygopro-database; \
  fi

FROM mono as builder

COPY . /windbot-source
WORKDIR /windbot-source
RUN msbuild WindBot.sln /p:Configuration=Release /p:TargetFrameworkVersion=v4.8 /p:OutDir=/windbot/

FROM mono
COPY --from=builder /windbot /windbot
COPY --from=resource-loader /resources/windbot /windbot

WORKDIR /windbot

EXPOSE 2399
ENTRYPOINT ["mono", "./WindBot.exe"]
CMD [ "ServerMode=true", "ServerPort=2399" ]
