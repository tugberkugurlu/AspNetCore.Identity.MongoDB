#!/bin/bash

for i in "$@"
do
case $i in
    -v=*|--version=*)
    BUILDVERSION="${i#*=}"
    shift # past argument=value
    ;;
    -c=*|--configuration=*)
    CONFIGURATION="${i#*=}"
    shift # past argument=value
    ;;
    -o=*|--outputFolder=*)
    OUTPUTFOLDER="${i#*=}"
    shift # past argument=value
    ;;
    --pack)
    PACK="YES"
    shift # past argument with no value
    ;;
    *)
    ;;
    --publish)
    PUBLISH="YES"
    shift # past argument with no value
    ;;
    *)
            # unknown option
    ;;
esac
done

echo "BUILD VERSION = ${BUILDVERSION}"
echo "CONFIGURATION = ${CONFIGURATION}"
echo "OUTPUT FOLDER = ${OUTPUTFOLDER}"
echo "PACK          = ${PACK}"
echo "PUBLISH       = ${PUBLISH}"

if [ -z ${CONFIGURATION+x} ]
then
    echo "No configuration is specified, defaulting to DEBUG"
    CONFIGURATION=DEBUG
fi

if [ -z ${BUILDVERSION+x} ]
then
    echo "No build version is specified, defaulting to 0.0.0"
    BUILDVERSION=0.0.0
fi

scriptsDir=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
artifactsDir=${scriptsDir%%/}/artifacts

if [ -z ${OUTPUTFOLDER+x} ]
then
    echo "No output folder is specified, defaulting to $artifactsDir"
    OUTPUTFOLDER=$artifactsDir
fi

outputDir=${OUTPUTFOLDER%%/}/apps
packagesOutputDir=${OUTPUTFOLDER%%/}/packages
testProjectRootDirectories=(${scriptsDir%%/}/tests/*/)
projectRootDirectories=(${scriptsDir%%/}/src/*/ ${scriptsDir%%/}/tests/*/)

# clean OUTPUTFOLDER
if [ -d "$OUTPUTFOLDER" ]; then
  rm -rf "$OUTPUTFOLDER"
fi

# restore
for projectDirectory in "${projectRootDirectories[@]}"
do
    # restore
    echo "starting to restore for $projectDirectory"
    dotnet restore $projectDirectory || exit 1
done

# build, publish
for projectDirectory in "${projectRootDirectories[@]}"
do
    projectFilePath="${projectDirectory%%/}/AspNetCore.Identity.MongoDB.csproj"

    # build
    echo "starting to build $projectFilePath"
    dotnet build $projectFilePath --configuration $CONFIGURATION || exit 1

    # publish
    echo "checking if $projectFilePath is publisable"
    if cat $projectFilePath | grep '"emitEntryPoint": true' &>/dev/null
    then
        if [ -z ${PUBLISH+x} ]
        then
            echo "$projectDirectory is publishable but publish is disabled"
        else
            echo "starting to publish for $projectDirectory"
            dotnet publish $projectDirectory --configuration $CONFIGURATION --output $outputDir --runtime active --no-build || exit 1
        fi
    else
        echo "$projectFilePath is not publisable. Looking to see if it should be packed"
        if [ -z ${PACK+x} ]
        then
            echo "Pack is disabled. Skipping pack on $projectDirectory"
        else
            echo "starting to pack for $projectDirectory"
            dotnet pack $projectDirectory --configuration $CONFIGURATION --output $packagesOutputDir --no-build || exit 1
        fi
    fi
done

for projectDirectory in "${testProjectRootDirectories[@]}"
do
    projectFilePath="${projectDirectory%%/}/AspNetCore.Identity.MongoDB.Tests.csproj"

    # test
    echo "starting to test $projectFilePath for configration $CONFIGURATION"
    dotnet test $projectFilePath --configuration $CONFIGURATION --no-build || exit 1
done