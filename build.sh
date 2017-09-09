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
testProjectRootDirectories=(${scriptsDir%%/}/tests/*/)

dotnet restore $scriptsDir || exit 1
dotnet clean $scriptsDir --configuration $CONFIGURATION --verbosity normal || exit 1
dotnet build $scriptsDir --configuration $CONFIGURATION --verbosity normal /property:VersionPrefix=$BUILDVERSION || exit 1

for projectDirectory in "${testProjectRootDirectories[@]}"
do
    projectFilePath="${projectDirectory%%/}/*.csproj"
    numberOfCsprojFiles="${#projectFilePath[@]}"
    if [ $numberOfCsprojFiles -eq "1" ]
    then
        if cat $projectFilePath | grep 'DotNetCliToolReference' | grep 'xunit' &>/dev/null
        then
            testDirectoryName=$(dirname "${projectFilePath}")
            echo "starting to test $testDirectoryName for configration $CONFIGURATION"
            (cd $testDirectoryName && dotnet xunit -nocolor -verbose -internaldiagnostics -configuration $CONFIGURATION) || exit 1
        else
            echo "$projectFilePath is not testable, skipping"
        fi
    else
        echo "There are $numberOfCsprojFiles csproj file(s) for $projectFilePath, skipping testing that"
    fi
done

if [ -z ${PACK+x} ]
then
    echo "Pack is disabled and the pack process is skipped"
else
    echo "starting to pack under $scriptsDir"
    dotnet pack $scriptsDir --no-build --configuration $CONFIGURATION --verbosity normal /property:VersionPrefix=$BUILDVERSION || exit 1
fi