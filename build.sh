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
            # unknown option
    ;;
esac
done

echo "BUILD VERSION = ${BUILDVERSION}"
echo "CONFIGURATION = ${CONFIGURATION}"
echo "OUTPUT FOLDER = ${OUTPUTFOLDER}"
echo "PACK          = ${PACK}"

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

echo "BUILD VERSION = ${BUILDVERSION}"
echo "CONFIGURATION = ${CONFIGURATION}"
echo "OUTPUT FOLDER = ${OUTPUTFOLDER}"
echo "PACK          = ${PACK}"

outputDir=${OUTPUTFOLDER%%/}/apps
projectDirectories=${scriptsDir%%/}/src/*/
projectJsonFiles=${projectDirectories%%/}/project.json

# clean OUTPUTFOLDER
if [ -d "$OUTPUTFOLDER" ]; then
  rm -rf "$OUTPUTFOLDER"
fi

# restore
for projectFilePath in $projectJsonFiles
do
    projectDirectory=$(dirname "${projectFilePath}")

    # restore
    echo "starting to restore for $projectDirectory"
    dnu restore $projectDirectory || exit 1
done

# build, publish
for projectFilePath in $projectJsonFiles
do
    projectDirectory=$(dirname "${projectFilePath}")

    # build
    echo "starting to build $projectFilePath"
    dnu build $projectFilePath --configuration $CONFIGURATION --out $OUTPUTFOLDER || exit 1

    # publish
    echo "checking if $projectFilePath is publisable"
    if cat $projectFilePath | grep '"emitEntryPoint": true' &>/dev/null
    then
        if [ -z ${PACK+x} ]
        then
            echo "$projectDirectory is publishable but pack is disabled"
        else
            echo "starting to publish for $projectDirectory"
            dnu publish $projectDirectory --configuration $CONFIGURATION --out $outputDir --runtime active || exit 1    
        fi
    fi
done