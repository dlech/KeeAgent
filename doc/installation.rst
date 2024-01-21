============
Installation
============

KeeAgent, like any other KeePass 2.x plugin, is installed by copying the ``.plgx``
file to the ``Plugins`` folder of the KeePass 2.x installation. The location
of this folder depends on how you installed KeePass 2.x (e.g. using an installer
or using the portable version).


Windows
=======

.. tip:: Users of the Chocolatey package manager may find a `KeeAgent package`__
         there (maintained by a 3rd party).

.. __: https://chocolatey.org/packages/keepass-plugin-keeagent


1.  Download KeeAgent from https://lechnology.com/software/keeagent

2.  Open the zip file

    .. figure:: images/win10-keeagent-zip-contents.png
        :alt: screenshot of Windows Explorer showing the contents of the
            downloaded KeeAgent zip file

3.  In another Explorer window, open the KeePass 2.x Plugins folder.

    .. figure:: images/win10-keepass2-plugins-folder.png
        :alt: screenshot of Windows Explorer showing KeePass 2.x Plugins directory

    .. note:: If you used the Windows installer to install KeePass, then the folder
        will be ``C:\Program Files\KeePass Password Safe 2\Plugins`` (or
        ``Program Files (x86)`` for older versions of KeePass).
            
        If you are using the portable version of KeePass 2.x, then hopefully
        you remember where you installed it.
  
4.  Drag and drop the ``KeeAgent.plgx`` file from the zip file to the KeePass 2.x
    Plugins folder, replacing the existing file if you have previously installed
    KeeAgent.

    .. figure:: images/win10-keepass2-plugins-folder-drag-and-drop.png
        :alt: screenshot of Windows Explorer showing dropping a file into the
            KeePass 2.x Plugins directory


Linux
=====

Packaging for Linux distributions is maintained by 3rd parties.

Arch
----

There are several 3rd party AUR packages for KeeAgent.

And, you may find something about `KeeAgent on the Arch Wiki`__.

.. __: https://wiki.archlinux.org/index.php/SSH_keys#KeePass2_with_KeeAgent_plugin


Everyone Else
-------------

This assumes that you already have KeePass 2.x (and therefore Mono) already
successfully installed. If you have not already, install the ``mono-complete``
package from your distro. Then extract ``KeeAgent.plgx`` from the zip file
and copy it to the ``Plugins`` folder where ``KeePass.exe`` is located. This
is usually at ``/usr/lib/keepass2/Plugins/`` unless you are using the portable
version of KeePass 2.x, in which case, hopefully you remember where you
installed it.


macOS
=====

On macOS, it is assumed that you are using the portable version of KeePass 2.x.

* Open the folder that contains ``KeePass.exe``.
* Drag and drop ``KeeAgent.plgx`` into the ``Plugins`` subdirectory.
* Restart KeePass if it was already running.
