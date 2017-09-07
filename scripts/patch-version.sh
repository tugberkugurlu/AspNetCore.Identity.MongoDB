# #!/bin/bash

# for i in "$@"
# do
# case $i in
    # -v=*|--version=*)
    # VERSIONNUMBER="${i#*=}"
    # shift # past argument=value
    # ;;
    # *)
            # # unknown option
    # ;;
# esac
# done

# echo "VERSION = ${VERSIONNUMBER}"

# if [ -z ${VERSIONNUMBER+x} ]
# then
    # echo "No version is specified, defaulting to 0.0.0"
    # VERSIONNUMBER=0.0.0
# fi

# scriptsDir=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
# rootDir="$(dirname "$scriptsDir")"
# projectFiles=$rootDir/src/**/project.json

# for projectFile in $projectFiles
# do
    # echo "patching $projectFile with version $VERSIONNUMBER"
    # echo $(jq ". + { \"version\": \"$VERSIONNUMBER\" }" <<<$(jq 'del(.version)' <<<"$(cat $projectFile)")) > $projectFile 
# done