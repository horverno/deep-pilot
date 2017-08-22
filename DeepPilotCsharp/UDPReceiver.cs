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
    /// An UDP message receiver class.
    /// </summary>
    class UDPReceiver
    {

        /// <summary>
        /// UDP message receiver object.
        /// </summary>
        private UdpClient listener;

        /// <summary>
        /// The address of the sender (ip and port pair).
        /// </summary>
        private IPEndPoint groupEP;

        /// <summary>
        /// Received message in string format.
        /// </summary>
        private string receivedData;

        /// <summary>
        /// Received message in raw byte array.
        /// </summary>
        private byte[] receivedByteArray;

        /// <summary>
        /// Delegate type for the message receive event.
        /// </summary>
        /// <param name="msg">Received message in a string.</param>
        public delegate void MessageReceive(string msg);

        /// <summary>
        /// This event activated when a message is arrived.
        /// </summary>
        public event MessageReceive MessageReceivedEvent;

        /// <summary>
        /// Create a new message receiver object.
        /// </summary>
        /// <param name="port">A specified port that want to listen.</param>
        public UDPReceiver(int port)
        {
            listener = new UdpClient(port);
            groupEP = new IPEndPoint(IPAddress.Any, port);
        }

        /// <summary>
        /// Receive one message in byte array. The method is wait until the message is arrived.
        /// </summary>
        /// <param name="data">An output array for the raw received message.</param>
        /// <returns>If true, the message arrived correctly.</returns>
        public Boolean ReceiveData(out byte[] data)
        {
            try
            {
                receivedByteArray = listener.Receive(ref groupEP);
                data = receivedByteArray;
                return true;
            }
            catch
            {
                data = null;
                return false;
            }
        }

        /// <summary>
        /// Receive one message in ascii string. The method is wait until the message is arrived.
        /// </summary>
        /// <param name="data">An output string for the encoded received message.</param>
        /// <returns>If true, the message arrived correctly.</returns>
        public Boolean ReceiveText(out string data)
        {
            try
            {
                receivedByteArray = listener.Receive(ref groupEP);
                receivedData = Encoding.ASCII.GetString(receivedByteArray, 0, receivedByteArray.Length);
                data = receivedData;
                return true;
            }
            catch
            {
                data = "";
                return false;
            }
        }

        /// <summary>
        /// Receive a string message continuously in asynchronous mode. When a message is arrives, the MessageReceivedEvent activated.
        /// </summary>
        public void ReceiveAsync()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var result = await listener.ReceiveAsync();
                    receivedData = Encoding.ASCII.GetString(result.Buffer);
                    MessageReceivedEvent(receivedData);
                }
            });
        }

    }
}
