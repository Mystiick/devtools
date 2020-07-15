#!/bin/bash

# Tells bash to not run pipe commands in a separate process
# Lets you update $branches below in a pipe command, and not lose the data
shopt -s lastpipe

# Color chart from https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
#	Black        0;30     Dark Gray     1;30
#	Red          0;31     Light Red     1;31
#	Green        0;32     Light Green   1;32
#	Brown/Orange 0;33     Yellow        1;33
#	Blue         0;34     Light Blue    1;34
#	Purple       0;35     Light Purple  1;35
#	Cyan         0;36     Light Cyan    1;36
#	Light Gray   0;37     White         1;37
YELLOW='\033[1;33m'
RED='\033[1;31m'
GREEN='\033[0;36m'
NC='\033[0m'

repo="";
clean="N";
all_repos="N";
search_commits="N";
force="N";
declare -a ALL_BRANCHES=();

### Parameters
###		$1: Directory to clean
function cleanRepo() {
	if [ -z "$1" ]; then
		printf "No Repo Specified"
	else
		cd $1
		printf "\n\nCleaning repo: ${YELLOW}"
		pwd
		printf "${NC}"
		
		declare -a branches=();
		
		git for-each-ref refs/heads/ --format="%(upstream:track) %(refname:short) %(HEAD)" | grep '^\[gone\].*' | {
			while read -r line; do
				#grep uses regex to find all lines that contain [gone]. Put them in the branches array
				branches+=("$line");
			done
		}
		
		if [ "${#branches[@]}" -gt 0 ]; then
			printf "${GREEN}Here are the branches that will be deleted${NC}\n"
			for b in "${branches[@]}"
			do
				echo "${b}"
			done
			
			printf "\n${RED}Do you want to delete the above branches? [Y/N]:${NC} "
			read proceed;
			
			if [ "$proceed" == "Y" ] || [ "$proceed" == "y" ] || [ "$proceed" == "Yes" ] || [ "$proceed" == "yes" ] || [ "$proceed" == "YES" ]; then
				for b in "${branches[@]}"
				do
					cleanBranch "$b"
				done
			fi
		else
			printf "${GREEN}No branches to delete${NC}\n"
		fi
	fi
}

### Parameters
###		$1: Branch name to clean
function cleanBranch() {
	read -ra SPLIT <<< "$1"		# String.split on " " and parse out the array beloow
	local tracked=${SPLIT[0]};	# Since we're greping for [gone], this should always be "[gone]"
	local branch=${SPLIT[1]};	# Branch name
	local head=${SPLIT[2]};		# Prints out a "*" if it is the current checked out branch
	
	if [ -n "$head" ]; then
		git checkout master
	fi
	
	if [ "$force" == "Y" ]; then
		git branch -D $branch	# Capital D to force delete
	else
		git branch -d $branch 	# Lower case d to normally delete
	fi
}

### Parameters
###		$1: Directory the repo sits in
function fetchRepo() {
	if [ -z "$1" ]; then
		printf "No Repository Specified";
	else
		cd $1
		printf "Fecthing repository: ${YELLOW}"
		pwd 	# Print working directory
		printf "${NC}"
		
		git fetch
	fi
}

### Parameters
###		$1: Directory the repo sits in
function searchForCommits() {
	if [ -z "$1" ]; then
		printf "No Repository Specified";
	else
		cd $1
		printf "Searching repository: ${YELLOW}"
		pwd 	# Print working directory
		printf "${NC}"
		
		git for-each-ref refs/heads/ --format="%(upstream:track) %(refname:short))" | grep '^\[.*ahead.*].*' | while read -r line; do echo $line; done
	fi
}

### Parameters
###		$1: Funciton to call
### Summary
###		Common function used to either iterate all repos, or just call the $1 function if the $repo folder
function processCommand() {
	if [ "$all_repos" == "Y" ]; then
		for dir in ../*/.git
		do
			# Call the $1 function for this repo. Need to specify /.. since it matches the /.git folder
			$1 "$dir"/..
		done
	else
		# Call function for specified repo
		$1 "../$repo"
	fi
}

###	Called when no parameters are passed in, or -h is passed in
function printHelp() {
	if [ "$force" == "Y" ]; then printf "${RED}"; fi;
	printf "Usage: ./git-loop.sh [OPTION]\n"
	printf "Loops over a repo's branches to prune, pull, or find missing commits\n"
	printf "Example:  ./git-loop.sh -c FolderName\n\n"
	printf "Options:\n"
	printf "\t-c [FOLDER_NAME]\tClean a specified repo at location ../[FOLDER_NAME]\n"
	printf "\t-C\t\t\tCleans ALL repos in the ../ path relative to the script\n"
	printf "\t-s [FOLDER_NAME]\tFind all branches with commits that have not been pushed yet\n"
	printf "\t-S\t\t\tFinds all unpushed commits in all repos relative to ../\n"
	printf "\n\t-f\t\t\tForces the action to take place, if applicable\n"
	printf "\t-h\t\t\tPrint this help text\n${NC}"
}

#Process arguments
while getopts "hc:fCs:Sp:P" args
do
	case $args in
		h) phelp="Y" ;;
		c) clean="Y";repo=$OPTARG ;;
		C) clean="Y";all_repos="Y" ;;
		f) force="Y"; ;;
		s) search_commits="Y";repo=$OPTARG ;;
		S) search_commits="Y";all_repos="Y"; ;;
		p) echo "Not yet implemented" ;;
		P) echo "Not yet implemented" ;;
	esac
done

if [ "$phelp" == "Y" ]; then
	printHelp
elif [ "$clean" == "Y" ]; then
	processCommand fetchRepo
	processCommand cleanRepo
elif [ "$search_commits" == "Y" ]; then
	processCommand searchForCommits
else
	printHelp
fi