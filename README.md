VMWareTasks
===========

![vmwaretasks](https://github.com/dblock/vmwaretasks/raw/master/Documentation/Images/VMWareLogo.jpg "VMWareTasks")

The VMWare SDK, specifically VixCOM, offers a rich set of programmable interfaces that enable developers to drive virtual machines programmatically with an asynchronous, job-based programming model. Unfortunately that turns out to be too complicated for most scenarios where developers want to use a simple object-oriented interface for common VMWare virtual machine automation tasks. The VMWareTasks is a commercial-grade library that implements this simple interface and makes programming against virtual machines a no-brainer. 

VMWareTasks contains a complete VixCOM wrapper Vestris.VMWareLib (Vestris.VMWareLib.dll) and a library Vestris.VMWareLib.Tools (Vestris.VMWareTools.dll) that implements additional commonly needed functionality or addresses known VixCOM API limitations. 

VMWareTasks exposes a C# interface, a COM-enabled interface for script clients, a Java bridge for Java programs and a set of MSBuild tasks.

Resources
---------

* [Need Help?](http://groups.google.com/group/vmwaretasks)
* [Latest Stable Release - 1.6](https://github.com/downloads/dblock/vmwaretasks/VMWareTasks-1.6.zip)

Prerequisites
-------------

In order to use the library you must install the following VMWare software. 

* VMWare VIX. This is the SDK, obtained from [http://www.vmware.com/download/sdk/vmauto.html](http://www.vmware.com/download/sdk/vmauto.html). Version 1.6.2 or newer is required for VI support. Version 1.8.0 or newer is required for VMWare Player support. 
* Either VMWare Workstation 6.5.2, 7.0 or 7.1, VMWare Server 2.0, VMWare Player 3.0 or 3.1, a Virtual Infrastructure environment (eg. ESXi) or VSphere 4.0 or 4.1. 


Getting Started (C#)
--------------------

Download the latest version of this library [here](https://github.com/downloads/dblock/vmwaretasks/). Add a reference to `Vestris.VMWareLib.dll` to your project and a `using`. 

``` csharp
using Vestris.VMWareLib;
```

You can now connect to a local VMWare Workstation, local or remote VMWare Server or a remote ESX server and perform VMWare client and server tasks. The following example creates, restores, powers on and removes a snapshot on a VMWare Workstation host. 

``` csharp
// declare a virtual host
using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
{
    // connect to a local VMWare Workstation virtual host
    virtualHost.ConnectToVMWareWorkstation();
    // open an existing virtual machine
    using (VMWareVirtualMachine virtualMachine = virtualHost.Open(@"C:\Virtual Machines\xp\xp.vmx"))
    {
        // power on this virtual machine
        virtualMachine.PowerOn();
        // wait for VMWare Tools
        virtualMachine.WaitForToolsInGuest();
        // login to the virtual machine
        virtualMachine.LoginInGuest("Administrator", "password");
        // run notepad
        virtualMachine.RunProgramInGuest("notepad.exe", string.Empty);
        // create a new snapshot
        string name = "New Snapshot";
        // take a snapshot at the current state
        VMWareSnapshot createdSnapshot = virtualMachine.Snapshots.CreateSnapshot(name, "test snapshot");
        createdSnapshot.Dispose();
        // power off
        virtualMachine.PowerOff();
        // find the newly created snapshot
        using (VMWareSnapshot foundSnapshot = virtualMachine.Snapshots.GetNamedSnapshot(name))
        {
            // revert to the new snapshot
            foundSnapshot.RevertToSnapshot();
            // delete snapshot
            foundSnapshot.RemoveSnapshot();
        }
    }
}
```

The following example creates, restores, powers on and removes a snapshot on a local VMWare Server 2.x host. VMWare Server 2.x generally behaves like an ESX host, replace "localhost" with a real host name to make a remote connection. 

``` csharp
// declare a virtual host
using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
{
    // connect to a local VMWare Server 2.x virtual host
    virtualHost.ConnectToVMWareVIServer("localhost:8333", "vmuser", "password");
    // open an existing virtual machine
    using (VMWareVirtualMachine virtualMachine = virtualHost.Open(@"[standard] xp/xp.vmx"))
    {
        // power on this virtual machine
        virtualMachine.PowerOn();
        // wait for VMWare Tools
        virtualMachine.WaitForToolsInGuest();
        // login to the virtual machine
        virtualMachine.LoginInGuest("Administrator", "password");
        // run notepad
        virtualMachine.RunProgramInGuest("notepad.exe", string.Empty);
        // create a new snapshot
        string name = "New Snapshot";
        // take a snapshot at the current state
        virtualMachine.Snapshots.CreateSnapshot(name, "test snapshot");
        // power off
        virtualMachine.PowerOff();
        // find the newly created snapshot
        using (VMWareSnapshot snapshot = virtualMachine.Snapshots.GetNamedSnapshot(name))
        {
            // revert to the new snapshot
            snapshot.RevertToSnapshot();
            // delete snapshot
            snapshot.RemoveSnapshot();
        }
    }
}
```

The following example creates, restores, powers on and removes a snapshot on a remote VMWare ESX host. 

``` csharp
// declare a virtual host
using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
{
    // connect to a remove (VMWare ESX) virtual machine
    virtualHost.ConnectToVMWareVIServer("esx.mycompany.com", "vmuser", "password");
    // open an existing virtual machine
    using (VMWareVirtualMachine virtualMachine = virtualHost.Open("[storage] testvm/testvm.vmx"))
    {
        // power on this virtual machine
        virtualMachine.PowerOn();
        // wait for VMWare Tools
        virtualMachine.WaitForToolsInGuest();
        // login to the virtual machine
        virtualMachine.LoginInGuest("Administrator", "password");
        // run notepad
        virtualMachine.RunProgramInGuest("notepad.exe", string.Empty);
        // create a new snapshot
        string name = "New Snapshot";
        // take a snapshot at the current state
        virtualMachine.Snapshots.CreateSnapshot(name, "test snapshot");
        // power off
        virtualMachine.PowerOff();
        // find the newly created snapshot
        using (VMWareSnapshot snapshot = virtualMachine.Snapshots.GetNamedSnapshot(name))
        {
            // revert to the new snapshot
            snapshot.RevertToSnapshot();
            // delete snapshot
            snapshot.RemoveSnapshot();
        }
    }
}
```

Note, that because VMWare VixCOM is a native 32-bit application, ensure that the platform target for your program is *x86* and not *Any CPU*. 
 
Most VMWareTasks objects are IDisposable and should be wrapped in a using construct or properly disposed of before calling `VMWareVirtualHost::Disconnect`. Failure to dispose of all objects, including snapshots and hosts may result in an access violation when VixCOM.dll is unloaded. This is particularly true when working with VMWare ESX 4. 

Contributing
------------

Fork the project. Make your feature addition or bug fix with tests. Send a pull request. Bonus points for topic branches.

Copyright and License
---------------------

MIT License, see [LICENSE](https://github.com/dblock/vmwaretasks/blob/master/LICENSE.md) for details.

(c) 2009-2012 [Daniel Doubrovkine & Vestris Inc.](http://code.dblock.org) and [Contributors](https://github.com/dblock/vmwaretasks/blob/master/HISTORY.md)




