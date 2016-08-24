#!/bin/bash

baseVersion=0.0.0-0
if semver "ignorethis" $(git tag -l) &>/dev/null
then
    baseVersion=$((semver $(git tag -l)) | tail -n1)
fi

if [ -z "$TRAVIS_TAG" ];
then
    if [ -z "$TRAVIS_BRANCH" ];
    then
        # can add the build metadata to indicate this is pull request build
        echo export PROJECT_BUILD_VERSION="$baseVersion-$TRAVIS_BUILD_NUMBER";
    else
        # can add the build metadata to indicate this is a branch build
        echo export PROJECT_BUILD_VERSION="$baseVersion-$TRAVIS_BUILD_NUMBER";
    fi
else 
    if ! semver $TRAVIS_TAG &>/dev/null
    then
        # can add the build metadata to indicate this is a tag build which is not a SemVer
        echo export PROJECT_BUILD_VERSION="$baseVersion-$TRAVIS_BUILD_NUMBER";
    else
        echo export PROJECT_BUILD_VERSION=$(semver $TRAVIS_TAG);
    fi 
fi