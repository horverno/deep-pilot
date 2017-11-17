using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace DeepPilotCsharp
{

    /// <summary>
    /// An UDP message sender class.
    /// </summary>
    public class UDPSender
    {

        /// <summary>
        /// UDP message sender object.
        /// </summary>
        private Socket sendingSocket;

        /// <summary>
        /// IP address of the recipient.
        /// </summary>
        private IPAddress sendToAddress;

        /// <summary>
        /// The address of the recipient (ip and port pair).
        /// </summary>
        private IPEndPoint sendingEndPoint;


        /// <summary>
        /// Create a new message sender object.
        /// </summary>
        /// <param name="address">IP of the recipient.</param>
        /// <param name="port">Port of the recipient.</param>
        public UDPSender(string address, int port)
        {
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendToAddress = IPAddress.Parse(address);
            sendingEndPoint = new IPEndPoint(sendToAddress, port);
        }

        /// <summary>
        /// Send a text message for the specified recipient.
        /// </summary>
        /// <param name="text">The message in string format.</param>
        /// <returns>If true, the message sent correctly.</returns>
        public Boolean SendText(string text)
        {
            byte[] send_buffer = Encoding.ASCII.GetBytes(text);
            try
            {
                sendingSocket.SendTo(send_buffer, sendingEndPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            sendingSocket.Close();
        }
    }
}
