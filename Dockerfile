ARG GIT_IMAGE=alpine/git:v2.52.0
ARG ALPINE_IMAGE=alpine:3.22
ARG NO_RESOURCE=0
ARG YGOPRO_DATABASE_REPO=https://code.moenext.com/nanahira/ygopro-database
ARG YGOPRO_DATABASE_BRANCH=master
ARG TARGET_FRAMEWORK_VERSION=v4.8

FROM ${ALPINE_IMAGE} AS mono-alpine
RUN apk add --no-cache mono

FROM ${GIT_IMAGE} AS resource-loader
ARG NO_RESOURCE
ARG YGOPRO_DATABASE_REPO
ARG YGOPRO_DATABASE_BRANCH
RUN mkdir -p /resources/windbot && \
  if [ "$NO_RESOURCE" != "1" ]; then \
    git clone --branch="${YGOPRO_DATABASE_BRANCH}" --depth=1 "${YGOPRO_DATABASE_REPO}" /ygopro-database && \
    cp -f /ygopro-database/locales/zh-CN/cards.cdb /resources/windbot/ && \
    rm -rf /ygopro-database; \
  fi

FROM mono-alpine AS builder
ARG TARGET_FRAMEWORK_VERSION

COPY . /windbot-source
WORKDIR /windbot-source
RUN xbuild WindBot.sln /p:Configuration=Release /p:TargetFrameworkVersion="${TARGET_FRAMEWORK_VERSION}" /p:OutDir=/windbot/

FROM mono-alpine
COPY --from=builder /windbot /windbot
COPY --from=resource-loader /resources/windbot /windbot

WORKDIR /windbot

EXPOSE 2399
ENTRYPOINT ["mono", "./WindBot.exe"]
CMD [ "ServerMode=true", "ServerPort=2399" ]
