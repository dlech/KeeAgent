# Building docs locally

## Prerequisites

- [Python 3.x](https://python.org)
- [Poetry](https://python-poetry.org)

## Commands

### Windows

    cd doc
    poetry install # one time setup of virtual environment
    poetry run make.bat html # build docs
    Invoke-Item .\_build\html\index.html # open in browser

### Linux

    cd doc
    poetry install # one time setup of virtual environment
    poetry run make html # build docs
    xdg-open _build/html/index.html # open in browser
