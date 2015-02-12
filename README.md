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

#### Prerequisites
* [Mono for Windows](http://www.go-mono.com/mono-downloads/download.html)
    * Once this is installed, add Mono.Posix to the GAC (using the Visual Studio Developer Command Prompt - run as administrator)

            gacutil -i <path-to-mono-install>\lib\mono\4.0\Mono.Posix.dll

    * Restart Visual Studio if needed
* KeePassPluginDevTools

#### Setting up the KeeAgent solution in Visual Studio

##### Create Local.proj file

##### Setup Debug Properties

These are not saved in KeeAgent.csproj, so you have to manually set them up.
* Open the project properties for the KeeAgent project.
* On the *Debug* tab, in the *Start Action* section, select *Start external program:* and enter `<path-to-project>/bin/Debug/KeePass.exe`, where *<path-to-project>* is the actual path on your machine.

   If you have not tried to build the project yet, then you will get an error that the program does not exist. Ignore the error.
* Then in the *Start Options* section, set *Command line arguments to `--debug --pw:test Test.kdbx`.
* Do the same for the *Release* and *ReleasePlgx* configurations, substituting the configuration name for *Debug* in `bin/Debug/KeePass.exe`.

    Also leave out the `--debug` command line argument for these configurations.

### Building KeeAgent in Xamarin Studio (Linux/Mac)

* Suggest that you use at Monodevelop >= 4.0
* Make sure you are using the OpenSSH SSH agent and not the GNOME Keyring SSH agent. [Instructions here](http://lechnology.com/software/keeagent/installation/#disable-ssh-component-of-gnome-keyring).
* Get my KeePass plugin dev tools.
    * On Ubuntu and derivatives, you can download from my ppa:

            sudo apt-add-repository ppa:dlech/keepass2-plugin-dev
            sudo apt-get update
            sudo apt-get install keepass2-plugin-dev

    * Other platforms, you can download and build from `git://github.com/dlech/KeePassPluginDevTools`. Be sure to build the *Release* configuration and not *Debug*. Note: you will need to edit the `Local.proj` file mentioned below to specify the path where the binary files are.

* Get the code:

        git clone git://github.com/dlech/KeeAgent
        cd KeeAgent
        git submodule init
        git submodule update
        cp Local.proj.sample.linux Local.proj
        # You need to edit Local.proj unless you installed keepass2 and keepass2-plugin-dev from .deb packages
        mozroots --import --sync # this is so SSL works with nuget

* In monodevelop:
    * In Edit > Preferences... > Projects > Build, check the box that says *Compile the project using MSBuild/Xbuild*. In newer versions, this option is not there (it is set per project in the project options). You must restart Monodevelop after making this change.
    * Make sure you have the nuget addin installed.
        * Add `http://mrward.github.com/monodevelop-nuget-addin-repository/4.0/main.mrep` to the addin sources.
        * see https://github.com/mrward/monodevelop-nuget-addin for more info
    * Open the `KeeAgent.sln` file.
    * Expand `SshAgentLib` project and right-click on *References*. Select *Restore NuGet Packages*. This will download the BouncyCastle crypto library.
    * Close and re-open the solution if it still says that BouncyCastle is missing.
    * Should be good to build and run now.


COPYRIGHT
---------
(C) 2012-2014 David Lechner <david@lechnology.com>
