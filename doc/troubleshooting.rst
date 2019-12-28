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


Other issues
============

Read everything here and still having problems?

Report your issues on `GitHub`_ (preferred) or send me a `message`_.

.. _`GitHub`: https://github.com/dlech/keeagent/issues
.. _`message`: https://lechnology.com/contact
