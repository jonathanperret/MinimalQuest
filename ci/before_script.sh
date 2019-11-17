#!/usr/bin/env bash

set -e
set -x
mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
set +x

UPPERCASE_BUILD_TARGET=${BUILD_TARGET^^};

if [ $UPPERCASE_BUILD_TARGET = "ANDROID" ]
then
    if [ -n "$ANDROID_KEYSTORE_BASE64" ]
	then
        echo '$ANDROID_KEYSTORE_BASE64 found, decoding content into keystore.keystore'
        echo $ANDROID_KEYSTORE_BASE64 | base64 --decode > keystore.keystore
    else
        echo '$ANDROID_KEYSTORE_BASE64'" env var not found, building with Unity's default debug keystore"
    fi
fi

for v in ${!UNITY_LICENSE_CONTENT*}; do
    if [[ "${v%%_BASE64}" != "$v" ]]; then
        echo "Decoding $v to ${v%%_BASE64}"
        export "${v%%_BASE64}=$(echo ${!v} | base64 --decode)"
    fi
done

LICENSE="UNITY_LICENSE_CONTENT_"$UPPERCASE_BUILD_TARGET

if [ -z "${!LICENSE}" ]
then
    echo "$LICENSE env var not found, using default UNITY_LICENSE_CONTENT env var"
    LICENSE=UNITY_LICENSE_CONTENT
else
    echo "Using $LICENSE env var"
fi

if [ -z "${!LICENSE}" ]
then
    echo "No Unity license found! Make sure to set UNITY_LICENSE_CONTENT"
    exit 1
fi

echo "Writing $LICENSE to license file /root/.local/share/unity3d/Unity/Unity_lic.ulf"
echo "${!LICENSE}" | tr -d '\r' > /root/.local/share/unity3d/Unity/Unity_lic.ulf