' Example: Getting Started (Workstation)
' declare a Virtual Host
WSCript.Echo "Creating VMWareComLib.VMWareVirtualHost"
set virtualHost = CreateObject("VMWareComLib.VMWareVirtualHost")
' connect to a local VMWare Workstation virtual host
WSCript.Echo "Connecting to VMWare Workstation"
virtualHost.ConnectToVMWareWorkstation()
' open an existing virtual machine
WSCript.Echo "Opening a Virtual Machine"
set virtualMachine = virtualHost.Open("c:\Users\dblock\Virtual Machines\Windows XP Pro SP3 25GB\WinXP Pro SP3 25GB.vmx")
' power on this virtual machine
WSCript.Echo "Powering on a Virtual Machine"
virtualMachine.PowerOn()
' wait for VMWare Tools
WSCript.Echo "Waiting for VMWare Tools"
virtualMachine.WaitForToolsInGuest()
' login to the virtual machine
WSCript.Echo "Logging in"
virtualMachine.LoginInGuest "Administrator", "admin123"
' create a new snapshot
WSCript.Echo "Creating snapshot"
virtualMachine.Snapshots.CreateSnapshot "test", "a new test snapshot"
' power off
WSCript.Echo "Powering off"
virtualMachine.PowerOff()
' find the newly created snapshot
WSCript.Echo "Locating snapshot"
set snapshot = virtualMachine.Snapshots.GetNamedSnapshot("test")
' revert to the new snapshot
WSCript.Echo "Reverting to snapshot"
snapshot.RevertToSnapshot
' delete snapshot
WSCript.Echo "Deleting snapshot"
snapshot.RemoveSnapshot
