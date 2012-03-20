VMWareTasks
===========

The VMWare SDK, specifically VixCOM, offers a rich set of programmable interfaces that enable developers to drive virtual machines programmatically with an asynchronous, job-based programming model. Unfortunately that turns out to be too complicated for most scenarios where developers want to use a simple object-oriented interface for common VMWare virtual machine automation tasks. The VMWareTasks is a commercial-grade library that implements this simple interface and makes programming against virtual machines a no-brainer. 

VMWareTasks contains a complete VixCOM wrapper Vestris.VMWareLib (Vestris.VMWareLib.dll) and a library Vestris.VMWareLib.Tools (Vestris.VMWareTools.dll) that implements additional commonly needed functionality or addresses known VixCOM API limitations. 

VMWareTasks exposes a C# interface, a COM-enabled interface for script clients, a Java bridge for Java programs and a set of MSBuild tasks.

Resources
---------

* [Need Help?](http://groups.google.com/group/vmwaretasks)

Contributing
------------

Fork the project. Make your feature addition or bug fix with tests. Send a pull request. Bonus points for topic branches.

Copyright and License
---------------------

MIT License, see [LICENSE](https://github.com/dblock/vmwaretasks/blob/master/LICENSE.md) for details.

(c) 2009-2012 [Daniel Doubrovkine & Vestris Inc.](http://code.dblock.org) and [Contributors](https://github.com/dblock/vmwaretasks/blob/master/HISTORY.md)




