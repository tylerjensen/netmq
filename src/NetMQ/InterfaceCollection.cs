﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace NetMQ
{
    /// <summary>
    /// class InterfaceItem provides the properties Address and BroadcastAddress (both are an IPAddress).
    /// </summary>
    public class InterfaceItem
    {
        public InterfaceItem([NotNull] IPAddress address, [NotNull] IPAddress broadcastAddress)
        {
            Address = address;
            BroadcastAddress = broadcastAddress;
        }

        [NotNull] public IPAddress Address { get; private set; }
        [NotNull] public IPAddress BroadcastAddress { get; private set; }
    }

    /// <summary>
    /// This is a list of InterfaceItems, each of which has an Address and BroadcastAddress,
    /// which is derived from all of the Network Interfaces present on this host at the time an instance of this class is created.
    /// </summary>
    public class InterfaceCollection : IEnumerable<InterfaceItem>
    {
        private readonly List<InterfaceItem> m_interfaceItems;

        /// <summary>
        /// Create a new InterfaceCollection that contains a list of InterfaceItems derived from all of the Network Interfaces present on this host.
        /// </summary>
        public InterfaceCollection()
        {
            // Get an array of all NetworkInterfaces that are running, and are not loopback nor Point-to-Point Protocol (PPP).
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                            i.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            i.NetworkInterfaceType != NetworkInterfaceType.Ppp);

            // From that, get all the UnicastAddresses.
            var addresses = interfaces
                .SelectMany(i => i.GetIPProperties().UnicastAddresses
                                  .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork));

            // From that, compose our list of InterfaceItems each of which has the Address, and a computed broadcast-address.
            m_interfaceItems = new List<InterfaceItem>();

            foreach (var address in addresses)
            {
                byte[] broadcastBytes = address.Address.GetAddressBytes();
                byte[] mask = address.IPv4Mask.GetAddressBytes();

                broadcastBytes[0] |= (byte)~mask[0];
                broadcastBytes[1] |= (byte)~mask[1];
                broadcastBytes[2] |= (byte)~mask[2];
                broadcastBytes[3] |= (byte)~mask[3];

                IPAddress broadcastAddress = new IPAddress(broadcastBytes);

                m_interfaceItems.Add(new InterfaceItem(address.Address, broadcastAddress));
            }
        }

        public IEnumerator<InterfaceItem> GetEnumerator()
        {
            return m_interfaceItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_interfaceItems.GetEnumerator();
        }
    }
}
