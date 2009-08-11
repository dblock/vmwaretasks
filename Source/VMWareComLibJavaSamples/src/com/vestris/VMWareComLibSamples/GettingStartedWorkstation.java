package com.vestris.VMWareComLibSamples;

import com.vestris.vmwarecomlib.*;

import junit.framework.TestCase;

public class GettingStartedWorkstation extends TestCase {
	
	// Example: Getting Started (Workstation)
	public void testConnectToVMWareWorkstation() {
		System.out.println("Creating virtual host");
		IVMWareVirtualHost virtualHost = ClassFactory.createVMWareVirtualHost();
		assertFalse(virtualHost.isConnected());
		// connect to a local VMWare Workstation virtual host
		System.out.println("Connecting to VMWare Workstation");
		virtualHost.connectToVMWareWorkstation();
		// open an existing virtual machine
		IVMWareVirtualMachine virtualMachine = virtualHost.open(
				"c:\\Users\\dblock\\Virtual Machines\\Windows XP Pro SP3 25GB\\WinXP Pro SP3 25GB.vmx");
		// power on this virtual machine
		System.out.println("Powering on");
		virtualMachine.powerOn();
		// wait for VMWare Tools
		System.out.println( "Waiting for VMWare Tools");
		virtualMachine.waitForToolsInGuest();
		// login to the virtual machine
		System.out.println("Logging in");
		virtualMachine.loginInGuest("Administrator", "admin123");
		// create a new snapshot
		System.out.println("Creating snapshot");
		virtualMachine.snapshots().createSnapshot("test", "a new test snapshot");
		// power off
		System.out.println("Powering off");
		virtualMachine.powerOff();
		// find the newly created snapshot
		System.out.println("Locating snapshot");
		IVMWareSnapshot snapshot = virtualMachine.snapshots().getNamedSnapshot("test");
		// revert to the new snapshot
		System.out.println("Reverting to snapshot");
		snapshot.revertToSnapshot();
		// delete snapshot
		System.out.println("Deleting snapshot");
		snapshot.removeSnapshot();
		// done
		System.out.println("Done");		
	}	
}
