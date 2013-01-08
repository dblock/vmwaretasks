1.8 (TBD)
---------

*Misc*

* #7: VMWareTasks.proj's vixcom target needs to be updated to use Visual Studio 2005's tlbimp.exe - [@icnocop](https://github.com/icnocop)

1.7 (8/24/2012)
---------------

*Features*

* First release off [Github](http://github.com/dblock/vmwaretasks) - [@dblock](https://github.com/dblock).
* Added support for VixCOM 1.11. Record/Replay has been deprecated, see [VixCOM Release Notes](http://www.vmware.com/support/developer/vix-api/VIX-1.11-ReleaseNotes.html) - [@mikewalker](https://github.com/mikewalker).

*Misc*

* `Vestris.VMWareLib.Tools.Windows.Shell.RunCommandInGuest` does not swallow exceptions in the finally block - [@icnocop](https://github.com/icnocop).
* #10296: Using Sandcastle and JDK from Tools directory - [@icnocop](https://github.com/icnocop).

1.6 (2/11/2012)
---------------

*Features*

* #8352: Refactored `GuestProcesses` to return a `ProcessCollection` class that inherits from `Dictionary`. 
* Added a `FindProcess` method to `ProcessCollection`. 
* Added a `WaitForVMWareUserProcessInGuest` function that waits for the vmware user process to exist. 
* Added `WaitForVMwareUserProcess` MsBuild task. 
* Added `WaitForVMwareUserProcess` to `IVMWareVirtualMachine` COM interface. 
* Added snapshot support and `GetConfiguration` to `VMWareTest` in `VMWareLibUnitTests`.
* #1627 and #8267: The host, username, and password parameters are now required when connecting to a vmware esx/esxi/vi server and an `ArgumentException` is thrown if they are invalid.
* #8622: Catching and throwing `Exception` in `VMWareLib` to include more relevant details on error.
* #8357: Added ability to pass options and a timeout to the `Vestris.VMWareLib.Tools.Windows.Shell.RunCommandInGuest` method.
* #9691: Added generated xml documentation files to the zip download.

*Bugs*

* #251057: Added missing `VMWareComTools.dll`.

*Misc*

* Added the specified number of milliseconds to a thrown `TimeoutException`.

1.5 (11/3/2010)
---------------

*Features*

* Added support for VixCOM 1.8.1, 1.9 and 1.10. VMWareTasks works against all 1.6.2, 1.7.0, 1.8.0, 1.8.1, 1.9 and 1.10 versions.
* Added `Vestris.VMWareLib.MSBuildTasks.dll` that adds support for MSBuild with MSBuild tasks that implement most common functions.
* Added `Vestris.VMWareLib.VMWareRootSnapshotCollection.RemoveSnapshot` that accepts a configurable timeout.
* Added documentation on contributing to VMWareTasks, including how to setup a development environment.
* Added `Vestris.VMWareLib.VMWareVirtualMachine.RunProgramInGuest` that accepts a timeout, but no options.

*Interface Changes*

* The `VMWareVirtualMachine.OpenUrlInGuest` API has been marked obsolete. It will be removed in a future release.

*Bugs*

* 5138: `Tools.Windows.Shell.GetEnvironmentVariables` fails with a "File not found" error on some x64 systems where `%tmp%` resolves to a non-existent path.
* 214878: Library throws `RPC_E_WRONG_THREAD` in apartment-threaded scenarios.

1.4 (11/28/2009)
----------------

*Features*

* Added support for VixCOM 1.8.0. VMWareTasks works and tested against all 1.6.2, 1.7.0, 1.8.0 versions.
* Added support for VMWare Player. Requires VixCOM 1.8.0.

*Interface Changes*

* `VMWareSnapshotCollection`, `VMWareRootSnapshotCollection` and `VMWareSnapshot` are now `IDisposable`.
* `VMWareSharedFolderCollection` is now `IDisposable`.
* `VMWareRootSnapshotCollection.CreateSnapshot` now returns a `VMWareSnapshot` that needs to be disposed of.

*Bugs*

* `VMWareVirtualMachine.Snapshots.CreateSnapshot` AVs against ESX4.

1.3 (08/27/2009)
----------------

*Features*

* Added support for VixCOM 1.7.0. VMWareTasks works against both 1.6.2 and 1.7.0.
* Added a `VMWareComLib` and `VMWareComTools` that expose a full scriptable COM interface.
* Added a `vestris-vmwarecomlib.jar` and `vestris-vmwarecomtools.jar` that exposes a Com4J JNI wrapper for `VMWareComLib` and `VMWareComTools` to Java clients.

*Misc*

* All `VMWareLib.Tools` objects are now `IDisposable` and explicitly release reference to the virtual machine.
* All assemblies are strongly named and signed.
* The source in the release package can now be built without any changes.

*Bugs*

* `VMWareRootSnapshotCollection.CreateSnapshot` doesn't pass flags to the COM API.

*Interface Changes*

* `VMWareVirtualHost.ConnectToVMWareServer` takes an additional username and password. Pass blank values to connect to a local VMWare Server 1.x.

1.2 (04/13/2009)
----------------

Completed VixCOM API full support.

* 1626: `VMWareVirtualHost.Register` and `Unregister`.
* 1631: `VMWareVirtualMachine.InstallTools`.
* 1629: `VMWareVirtualMachine.Snapshots.Enabled`.
* 1630: `VMWareVirtualMachine.GetFileInfoInGuest`.
* 2688: `VMWareVirtualMachine.Clone` and `VMWareSnapshot.Clone`.
* 2691: `VMWareVirtualMachine.Delete`.
* 1628: `VMWareVirtualMachine.BeginRecording` and `EndRecording`, `VMWareSnapshot.BeginReplay` and `EndReplay`.
* 1633: `VMWareVirtualMachine.Reset`, `Suspend`, `Pause`, `UnPause`, `IsPaused` and `IsSuspended`.
* 1913: `VMWareVirtualHost.IsConnected`.
* 1634: `VMWareVirtualMachine.RunScriptInGuest`.
* 1632: `VMWareVirtualMachine.OpenUrlInGuest`.
* 1635: `VMWareVirtualMachine.UpgradeVirtualHardware`.

* Added an optional `VMWareTools` package built on top of `VMWareLib` that implements additional common VMWare tasks.
* `VMWareLib.Tools.GuestOS.IpAddress`: guest operating system IP address information.
* `VMWareLib.Tools.GuestOS.ReadFile`, `ReadFileLines` and `ReadFileBytes`: read remote files as binary data or text with encoding support.
* 2232: `VMWareLib.Tools.Windows.Shell.GetEnvironmentVariables`: obtain logged-in user environment.
* `VMWareLib.Tools.Windows.Shell.RunCommandInGuest`: runs commands and collect console output.
* `VMWareLib.Tools.Windows.MappedNetworkDrive`: maps guest operating system network resources.

*Interface changes*

* `VMWareVirtaulMachine.PowerOn` no longer calls `WaitFor`
* Renamed `VMWareVirtualMachine.Login` to `LoginInGuest` and renamed `VMWareVirtualMachine.Logout` to `LogoutFromGuest` to be consistent with VIX COM API and allow power-on without tools installed.

*Misc*

* Added `VMWareVirtualMachine.ShutdownGuest` and `PowerOff` that allow specifying shutdown parameters explicitly.
* Added support for VMWare Server with `VMWareVirtualMachine.ConnectToVMWareServer`.
* Exposed a `VMWareVirtualMachine.LoginInGuest` function with power options.
* Lots of new documentation with examples.

*Bugs*

* `GetNamedSnapshot` behaves differently on Workstation and VI, now throws exception when snapshot not found. Use `FindSnapshotByName` to get a null result when the snapshot doesn't exist.
* `KillProcessInGuest` unit test needs to wait for process to actually die according to VMWare docs.
* Removing a shared folder doesn't remove it from the collection when not the same object.
* Fixed different behavior of `ListDirectoryInGuest` between Workstation and VI.
* API-level errors aren't surfaced and the callback wait will never be set when VixCOM is not installed.

1.1 (02/12/2008)
----------------

* First release off [CodePlex](http://vmwaretasks.codeplex.com).
* Improved timeout model from busy to blocking job-based wait.
* Added support for snapshots with duplicate names.
* New Sandcastle-generated CHM documentation.
* Built with Visual Studio 2008, targeting .NET 2.0.

1.0
---

* [CodePlex Article on Automating VMWareTasks in C# with VIX API](http://www.codeproject.com/Articles/31961/Automating-VMWare-Tasks-in-C-with-the-VIX-API)
