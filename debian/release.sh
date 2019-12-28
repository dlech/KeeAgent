#!/bin/bash
#
# Maintainer script for publishing releases.

set -e

source=$(dpkg-parsechangelog -S Source)
version=$(dpkg-parsechangelog -S Version)

debuild -S
debuild -- clean

dput ppa:dlech/keepass2-plugins-beta ../${source}_${version}_source.changes

gbp buildpackage --git-tag-only
