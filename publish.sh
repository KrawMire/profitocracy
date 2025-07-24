#!/bin/bash

if [ $# != 1 ]; then
  echo "Usage: publish.sh [all|android|ios]"
  exit 1
fi

source .env

# Can be only one of the following: android, ios, both
MODE=$1

function publish_android() {
  echo "Starting publishing application for Android..."

  dotnet publish \
    src/Profitocracy.Mobile/Profitocracy.Mobile.csproj \
    -f net9.0-android \
    -c Release \
    -o "${ANDROID_PUBLISH_PATH}" \
    -p:AndroidKeyStore=true \
    -p:AndroidSigningKeyStore="${ANDROID_SIGNING_KEYSTORE}" \
    -p:AndroidSigningKeyAlias="${ANDROID_SIGNING_KEY_ALIAS}" \
    -p:AndroidSigningKeyPass="${ANDROID_SIGNING_KEY_PASS}" \
    -p:AndroidSigningStorePass="${ANDROID_SIGNING_STORE_PASS}"

  if [ $? -ne 0 ]; then
    echo "An error occurred while publishing Android application. Exiting..."
    exit 3
  fi

  echo "Application for Android has been successfully published to ${ANDROID_PUBLISH_PATH}."
}

function publish_ios() {
  echo "Starting publishing application for iOS..."

  dotnet publish \
    src/Profitocracy.Mobile/Profitocracy.Mobile.csproj \
    -f net9.0-ios \
    -c Release \
    -o "${IOS_PUBLISH_PATH}" \
    -p:ArchiveOnBuild=true \
    -p:RuntimeIdentifier="${IOS_RUNTIME_ID}" \
    -p:CodesignKey="${IOS_CODESIGN_KEY}" \
    -p:CodesignProvision="${IOS_CODESIGN_PROVISION}"

    if [ $? -ne 0 ]; then
      echo "An error occurred while publishing iOS application. Exiting..."
      exit 3
    fi

  echo "Application for iOS has been successfully published to ${IOS_PUBLISH_PATH}."
}

case $MODE in
  android)
    publish_android
    ;;

  ios)
    publish_ios
    ;;

  all)
    publish_android
    publish_ios
    ;;

  *)
    echo "Invalid publishing mode ${MODE}. Valid arguments: ios, android, all."
    exit 2
    ;;
esac
