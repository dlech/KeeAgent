#!/bin/bash

file_version=$(cat KeeAgent.sln | sed -ne 's/\s*version = \(.*\)/\1/p')
file_name=KeeAgent_v${file_version}.tar
git_exclude="--exclude .gitignore --exclude .gitmodules --exclude .gitattributes"

git ls-files | xargs tar --exclude SshAgentLib ${git_exclude} -cf ${file_name} packages/
cd SshAgentLib
git ls-files | xargs tar --transform 's|^|SshAgentLib/|' ${git_exclude}  -rf ../${file_name} packages/
cd ..
gzip -f ${file_name}
