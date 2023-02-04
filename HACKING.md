
### Building KeeAgent in Visual Studio (Windows)

#### Setting up the KeeAgent solution in Visual Studio

##### Setup Debug Properties

These are not saved in KeeAgent.csproj, so you have to manually set them up.
* Open the project properties for the KeeAgent project.
* On the *Debug* tab, in the *Start Action* section, select *Start external
  program:* and enter `<path-to-project>/bin/Debug/KeePass.exe`, where
  `<path-to-project>` is the actual path on your machine.

  If you have not tried to build the project yet, then you will get an error
  that the program does not exist. Ignore the error.
* Then in the *Start Options* section, set *Command line arguments* to
  `--debug --pw:test Test.kdbx`.
* Do the same for the *Release* and *ReleasePlgx* configurations, substituting
  the configuration name for *Debug* in `bin/Debug/KeePass.exe`.

  Also leave out the `--debug` command line argument for these configurations.


### Building KeeAgent in Visual Studio Code (Linux/MacOS/Windows)

* Get the code and open it in Visual Studio Code:

        git clone --recursive git@github.com:dlech/KeeAgent.git
        cd KeeAgent
        nuget restore
        code .

* Install suggested extensions in VS Code

* Run using VS Code debugger (<kbd>F5</kbd>)

If `msbuild` is not present, change `tasks.json` to us `xbuild` instead.


### Building KeeAgent from the Command Line (Linux/MacOS)

* Make sure you have mono (>= v3.2.x)

* Get the code:

        git clone --recursive git@github.com:dlech/KeeAgent.git
        cd KeeAgent

* Restore the nuget packages:

        wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
        mono nuget.exe restore

* And build:

        xbuild /property:Configuration=ReleasePlgx KeeAgent.sln

* The plgx file will be at `bin/ReleasePlgx/KeeAgent.plgx`.
