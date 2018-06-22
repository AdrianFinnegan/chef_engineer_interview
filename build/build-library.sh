#!/bin/bash
#
# Library of functions for build scripts
#
function write_start_component() {
    local name=$1
    printf "\n"
    read -p "Setup Dependent Component: ${name} (Y/n) ? " input
    if [ "$input" != 'n' ]; then
        printf "\n\nStarting Dependent Component: ${name}\n\n"
        return 0
    else
        return 1
    fi
}

function write_end_component() {
    printf "\n\nEnd Dependent Component\n"
}

function pull_or_clone() {
    local prjname=$1
    pushd $SCRIPT_PATH > /dev/null
    local dir=../$prjname
    if [ ! -d $dir ]; then
        read -p "Clone ${prjname} (Y/n)  ? " input
        if [ "$input" != 'n' ]; then
            echo "Cloning ${prjname}"
            pushd .. > /dev/null
            git clone git@git:$prjname
            pushd $prjname > /dev/null
            git checkout integration
            popd > /dev/null
            popd > /dev/null
        fi
    else
        read -p "Pull ${prjname} (Y/n) ? " input
        if [ "$input" != 'n' ]; then
            echo "Pulling ${prjname}"
            pushd $dir > /dev/null
            git pull
            popd > /dev/null
        fi
    fi
    popd > /dev/null
}

function build() {
    local prjname=$1
    local dir=../$prjname
    read -p "Build ${prjname} (Y/n) ? " input
    if [ "$input" == 'n' ]; then
        return
    fi

    echo "Building ${prjname}"
    pushd $SCRIPT_PATH > /dev/null
    pushd $dir > /dev/null

    if [ -f ./build.sh ]; then
        ./build.sh
    elif [ -f ./Development/Code/build.cmd ]; then
        pushd ./Development/Code > /dev/null
        execute_dos_command "$COMSPEC //c build.cmd"
        popd > /dev/null
    elif [ -d Scm/Build/Development ]; then
        pushd Scm/Build/Development > /dev/null
        nant compile
        popd > /dev/null
    fi

    popd > /dev/null
    popd > /dev/null
}

function devinstall() {
    local prjname=$1
    local dir=../$prjname
    read -p "DevInstall ${prjname} (Y/n) ? " input
    if [ "$input" != 'n' ]; then
        echo "DevInstall-ing ${prjname}"
        pushd $SCRIPT_PATH > /dev/null
        pushd $dir > /dev/null
        pwd
        if [ -f ./devinstall.sh ]; then
            ./devinstall.sh
        else
            pushd Scm/Build/Development
            nant devinstall
            popd > /dev/null
        fi
        popd > /dev/null
        popd > /dev/null
    fi
}

function get_dos_path_env_variable() {
    # Replace colons with spaces to create list.
    local fixedpath=
    local dospath
    local oldIFS="$IFS"
    IFS=':'
    for i in $PATH
    do
        if [ ! -d "$i" ]; then
            continue
        fi
        pushd "$i" > /dev/null
        # In Msys use "pwd -W" to get windows path
        dospath=$(pwd -W)

        #echo "$i -> $dospath"
        if [ "$fixedpath" != "" ]; then
            fixedpath="$fixedpath;$dospath"
        else
            fixedpath=$dospath
        fi

        popd > /dev/null
    done
    IFS="$oldIFS"
    echo $fixedpath
}

function execute_dos_command() {
    local FIXED_PATH=$(get_dos_path_env_variable)
    local SAVED_PATH=$PATH
    PATH=$FIXED_PATH
    local command=$1
    echo "Executing DOS command: $*"
    $*
    PATH=$SAVED_PATH
}
