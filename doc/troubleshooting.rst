===============
Troubleshooting
===============

Here are some suggestions for troubleshooting KeeAgent.


Common issues
=============

**Problem:**

    After upgrading my OS, my 1024-bit RSA key no longer works with KeeAgent in
    Client mode.

**Solution:**

    OpenSSH dropped support for RSA keys <= 1024-bit in v7.6 because they are
    no longer considered secure. This version of OpenSSH ships with Ubuntu 18.04
    and macOS High Sierra. Create a new, larger key.

**Problem:**

    Connecting to a server fails when too many keys are loaded in KeeAgent. For
    example, you might get an error message like this::

        Server sent disconnect message type 2 (protocol error): "Too many authentication failures for user"

    Many servers will only try the first 6 keys before returning an error. This
    number will vary from server to server though.

**Solution:**

    To work around this, limit the number of keys that load automatically, use
    the *Show selection dialog when a client program requests a list of keys*
    option or consider having the keys saved to a temporary file as described
    in :doc:`usage/tips-and-tricks`.

**Problem:**

When I start KeePass, I get an error message like this::

    The following plugin is incompatible with the current KeePass version:
    Have a look at the plugin's website for an appropriate version.

**Solution:**

Run KeePass.exe from a command prompt with the ``--debug`` option (and ``--saveplgxcr``
for KeePass versions older than 2.31). This should give more detailed information
on the cause of the error. Report the error using the link for other issues below.

**Problem:**

Keys will not load with the confirm constraint.

**Solution:**

On Windows, if you are using Pageant and running KeeAgent in client mode, then
constraints will not work. Pageant does not support constraints.

On Linux, if you are using a GNOME based desktop (includes Cinnamon, MATE, Unity,
others), follow the instructions for disabling the SSH agent in GNOME keyring.

**Problem:**

"The plugin cannot be loaded A newer .NET framework is required." error on Linux
and KeePass version is less than 2.52.

**Solution:**

Use workaround described in `issue #343 <https://github.com/dlech/KeeAgent/issues/343>`_.


Other issues
============

Read everything here and still having problems?

Report your issues on `GitHub`_ (preferred) or send me a `message`_.

.. _`GitHub`: https://github.com/dlech/keeagent/issues
.. _`message`: https://lechnology.com/contact


Debugging
=========

You can use Visual Studio to see a limited amount of debug information.

Prerequisite: `Visual Studio 2022 <https://visualstudio.microsoft.com/vs/>`_
with the *.NET desktop development* workload installed.

    .. figure:: images/vs-dotnet-desktop-development.png
        :alt: screenshot of Visual Studio Installer with .NET desktop development workload selected

1. Start *KeePass*.

2. Start *Visual Studio*.

3. Select *Continue without code*.

    .. figure:: images/vs-continue-without-code.png
        :alt: screenshot of Visual Studio 2022 start screen "Continue without code" hyperlink

4. Click the *Attach* button.

    .. figure:: images/vs-attach.png
        :alt: screenshot of Visual Studio 2022 start screen "Attach" button

5. Select *KeePass* from the list of processes. Use the filter to find it quickly.

    .. figure:: images/vs-attach-to-process-keepass.png
        :alt: screenshot of Visual Studio 2022 "Attach to Process" dialog with "KeePass.exe" selected.

6. Click the *Attach* button.

7. Select the *Output* tab.
    
        .. figure:: images/vs-output-tab.png
            :alt: screenshot of Visual Studio 2022 "Output" tab

8. Reproduce the issue in KeePass/KeeAgent.

9. Look for any useful information in the *Output* tab.
