#!/bin/bash

for i in "$@"
do
case $i in
    -k=*|--apikey=*)
    APIKEY="${i#*=}"
    shift # past argument=value
    ;;
    -v=*|--version=*)
    VERSION="${i#*=}"
    shift # past argument=value
    ;;
    *)
            # unknown option
    ;;
esac
done

if [ -z ${APIKEY+x} ]
then
    echo "Error: ApiKey needs to be specified through --apikey parameter"
    exit 1
fi

if [ -z ${VERSION+x} ]
then
    echo "Error: Version needs to be specified through --version parameter"
    exit 1
fi

scriptsDir=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
rootDir="$(dirname "$scriptsDir")"
nuGetVersion="3.2.0"
nuGetUrl="https://dist.nuget.org/win-x86-commandline/v$nuGetVersion/nuget.exe"
nugetExeDirPath=${rootDir%%/}/.nuget
nugetExePath=${nugetExeDirPath%%/}/nuget.exe
nugetPackagePath=$(find ./artifacts/ -regextype posix-extended -regex ".*MongoDB.$VERSION.nupkg")

if [ ! -f "$nugetExePath" ]
then
    if [ ! -d "$nugetExeDirPath" ]
    then
        mkdir $nugetExeDirPath || exit 1  
    fi

    echo "downloading '$nuGetUrl' into '$nugetExePath'..."
    curl -L -o $nugetExePath $nuGetUrl | tac || exit 1  
fi

echo "pushing $nugetPackagePath to NuGet.org..."
mono nuget.exe push $nugetPackagePath -ApiKey $APIKEY || exit 1