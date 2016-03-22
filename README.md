KeeAgent is a plugin for KeePass 2.x. It allows other programs to access SSH
keys stored in your KeePass database for authentication. It can either act as a
stand-alone agent or it can interface with an external agent.


USAGE
-----
See http://lechnology.com/software/keeagent

HACKING
-------

Pull requests are welcome!

### Building KeeAgent in Visual Studio (Windows)

#### Setting up the KeeAgent solution in Visual Studio

##### Setup Debug Properties

These are not saved in KeeAgent.csproj, so you have to manually set them up.
* Open the project properties for the KeeAgent project.
* On the *Debug* tab, in the *Start Action* section, select *Start external program:* and enter `<path-to-project>/bin/Debug/KeePass.exe`, where *<path-to-project>* is the actual path on your machine.

   If you have not tried to build the project yet, then you will get an error that the program does not exist. Ignore the error.
* Then in the *Start Options* section, set *Command line arguments to `--debug --pw:test Test.kdbx`.
* Do the same for the *Release* and *ReleasePlgx* configurations, substituting the configuration name for *Debug* in `bin/Debug/KeePass.exe`.

    Also leave out the `--debug` command line argument for these configurations.

### Building KeeAgent in Xamarin Studio (Linux/OS X)

* Suggest that you use at Monodevelop >= 5.0
* Make sure you are using the OpenSSH SSH agent and not the GNOME Keyring SSH agent. [Instructions here](http://lechnology.com/software/keeagent/installation/#disable-ssh-component-of-gnome-keyring).

* Get the code:

        git clone git://github.com/dlech/KeeAgent --recursive

* In monodevelop:
    * In Edit > Preferences... > Projects > Build, check the box that says *Compile the project using MSBuild/Xbuild*. In newer versions, this option is not there (it is set per project in the project options). You must restart Monodevelop after making this change.
    * If you are using an older version of MonoDevelop, make sure you have the nuget addin installed.
        * Add `http://mrward.github.com/monodevelop-nuget-addin-repository/4.0/main.mrep` to the addin sources.
        * see https://github.com/mrward/monodevelop-nuget-addin for more info
    * Open the `KeeAgent.sln` file.
    * Expand `SshAgentLib` project and right-click on *References*. Select *Restore NuGet Packages*.
    * Close and re-open the solution if it still says that BouncyCastle is missing.
    * Should be good to build and run now.

### Building KeeAgent from the Command Line (Linux/OS X)

* Make sure you have a fairly recent mono (>= v3.2.x)

* Get the code:

        git clone git://github.com/dlech/KeeAgent --recursive
        cd KeeAgent

* Restore the nuget packages:

        wget https://nuget.org/nuget.exe
        mono nuget.exe restore

* And build:

        xbuild /property:Configuration=ReleasePlgx KeeAgent.sln

* The plgx file will be at `bin/ReleasePlgx/KeeAgent.plgx`.

COPYRIGHT
---------
(C) 2012-2015 David Lechner <david@lechnology.com>
