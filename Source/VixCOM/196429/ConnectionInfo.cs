using System;
using System.Collections.Generic;
using System.Text;

namespace VMWareCrash
{
    public class ConnectionInfo
    {
        public int HostType;
        public string Uri; // eg. https://linc.nycapt35k.com/sdk
        public string Username; // eg. vmuser
        public string Password; // eg. admin123
        public string Vmx; // eg. "[welby] ddoub-red/ddoub-red.vmx"

        public ConnectionInfo(int hostType, string uri, string username, string password, string vmx)
        {
            HostType = hostType;
            Uri = uri;
            Username = username;
            Password = password;
            Vmx = vmx;
        }

        public ConnectionInfo(string vmx)
            : this(VixCOM.Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION, "", "", "", vmx)
        {

        }

        public ConnectionInfo(string uri, string username, string password, string vmx)
            : this(VixCOM.Constants.VIX_SERVICEPROVIDER_VMWARE_VI_SERVER, uri, username, password, vmx)
        {

        }
    }
}
