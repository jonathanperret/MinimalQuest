#!/usr/bin/env bash

set -e

: ${UNITY_LICENSE_CONTENT:?}
: ${BUILD_NAME:?}
: ${BUILD_TARGET:?}
: ${IMAGE_NAME:?}

mkdir -p Temp
tar czf Temp/project.tgz Assets/ ProjectSettings/ ci/

docker run \
  --rm -ti \
  -e UNITY_LICENSE_CONTENT \
  -e BUILD_NAME \
  -e BUILD_TARGET \
  -w /project/ \
  -v "$(pwd)"/Temp/project.tgz:/project.tgz:ro \
  -v "$(pwd)"/Builds:/project/Builds \
  -v "$(basename "$(pwd)")_unity_cache":/project/Library \
  $IMAGE_NAME \
  /bin/bash -c "tar xzf /project.tgz && /project/ci/before_script.sh && /project/ci/build.sh"