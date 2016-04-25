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
                        iplist.Add(new IP(IPInfo.Address, IPInfo.IPv4Mask));
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

        public sealed class IP {
            private IPAddress _ip, _mask, _broadcast;
            public IP(IPAddress Ip, IPAddress Mask) {
                _ip = Ip;
                _mask = Mask;
            }

            public IPAddress Ip { get { return _ip; } }
            public IPAddress Mask { get { return _mask; } }

            // Broadcast is calculated at first use
            public IPAddress Broadcast{
                get{
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
            } // Broadcast
        } // IP

    }
}
