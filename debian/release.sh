#!/bin/bash
#
# Maintainer script for publishing releases.

set -e

source=$(dpkg-parsechangelog -S Source)
version=$(dpkg-parsechangelog -S Version)

gbp export-orig --submodules

debuild -S --no-check-builddeps
debuild --no-check-builddeps -- clean

dput ppa:dlech/keepass2-plugins ../${source}_${version}_source.changes

gbp buildpackage --git-tag-only
