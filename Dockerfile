ARG YGOPRO_DATABASE_REPO=https://code.moenext.com/nanahira/ygopro-database
ARG YGOPRO_DATABASE_BRANCH=master

FROM debian:trixie-slim AS resource-loader
ARG NO_RESOURCE=0
ARG YGOPRO_DATABASE_REPO
ARG YGOPRO_DATABASE_BRANCH
RUN mkdir -p /resources/windbot && \
  if [ "$NO_RESOURCE" != "1" ]; then \
    apt update && apt -y install git && \
    git clone --branch="${YGOPRO_DATABASE_BRANCH}" --depth=1 "${YGOPRO_DATABASE_REPO}" /ygopro-database && \
    cp -f /ygopro-database/locales/zh-CN/cards.cdb /resources/windbot/ && \
    rm -rf /var/lib/apt/lists/* /ygopro-database; \
  fi

FROM mono AS builder

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
