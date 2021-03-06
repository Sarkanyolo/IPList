namespace IPList {
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public sealed class IPv4List : IEnumerable<IPv4List.IP> {
        private List<IP> iplist;

        public IPv4List() {
            iplist = new List<IP>();
            foreach (var IFace in NetworkInterface.GetAllNetworkInterfaces()) {
                foreach (var IPInfo in IFace.GetIPProperties().UnicastAddresses) {
                    if (IPInfo.Address.AddressFamily == AddressFamily.InterNetwork && !IPInfo.Address.ToString().StartsWith("127.")) {
                        iplist.Add(new IP(IPInfo.Address, IPInfo.IPv4Mask, IFace.GetPhysicalAddress()));
                    }
                }
            }
        }

        public int Count { get { return iplist.Count; } }
        public IP this[int key] { get { return iplist[key]; } private set { } }
        public IEnumerator<IP> GetEnumerator() {
            return iplist.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #region IP Class
        public sealed class IP {
            private IPAddress _ip, _mask, _broadcast;
            private PhysicalAddress _mac;
            public IP(IPAddress Ip, IPAddress Mask, PhysicalAddress Mac) {
                _ip = Ip;
                _mask = Mask;
                _mac = Mac;
            }

            public IPAddress Ip { get { return _ip; } private set { } }
            public IPAddress Mask { get { return _mask; } private set { } }
            public PhysicalAddress Mac { get { return _mac; } private set { } }

            // Broadcast is calculated at first use
            public IPAddress Broadcast {
                get {
                    if (_broadcast == null) {
                        byte[] BMask = new byte[4];
                        byte[] BIP = new byte[4];
                        for (int i = 0; i < 4; i++) {
                            BMask[i] = (byte)~(_mask.GetAddressBytes()[i]);
                            BIP[i] = (byte)((_ip.GetAddressBytes()[i]) | BMask[i]);
                        }
                        _broadcast = new IPAddress(BIP);
                    }
                    return _broadcast;
                }
            }
        }
        #endregion
    }
}
