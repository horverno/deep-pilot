using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeepPilotCsharp.Cam
{
    public class CamTCP
    {
        private const int MSG_SIZE = 8;
        private string _IPAddress;
        private int _port;
        private NetworkStream _stream = null;
        private TcpClient _client = null;

        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _ct;

        public delegate void GetImages(Bitmap image);
        public event GetImages Images;

        public CamTCP(string ip, int port)
        {
            _IPAddress = ip;
            _port = port;
            _client = new TcpClient();
            _cancelTokenSource = new CancellationTokenSource();
            _ct = _cancelTokenSource.Token;
        }

        public void DoWork()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                try
                {
                    _client.Connect(_IPAddress, _port);
                    _stream = _client.GetStream();
                    byte[] PictureData;
                    byte[] MsgData;
                    int PictureSize;
                    while (true)
                    {
                        if (_stream.DataAvailable)
                        {
                            if (_ct.IsCancellationRequested)
                            {
                                _stream.Close();
                                _client.Close();
                                return;
                            }
                            do
                            {
                                if (_ct.IsCancellationRequested)
                                {
                                    _stream.Close();
                                    _client.Close();
                                    return;
                                }
                                MsgData = new byte[MSG_SIZE];
                                _stream.Read(MsgData, 0, MSG_SIZE);
                                MsgData = RemoveNullElement(MsgData);

                                if (_ct.IsCancellationRequested)
                                {
                                    _stream.Close();
                                    _client.Close();
                                    return;
                                }

                                PictureSize = int.Parse(Encoding.ASCII.GetString(MsgData));
                                PictureData = new byte[PictureSize];
                                int ossz = PictureSize;
                                int tolt = 0;
                                int akt = 0;
                                MemoryStream ms = new MemoryStream();
                                while (tolt != ossz)
                                {
                                    akt = _stream.Read(PictureData, 0, ossz);
                                    tolt += akt;
                                    ms.Write(PictureData, 0, akt);
                                    if (_ct.IsCancellationRequested)
                                    {
                                        _stream.Close();
                                        _client.Close();
                                        return;
                                    }
                                }
                                Images?.Invoke(new Bitmap(ms, true));
                                ms.Close();
                            } while (_stream.DataAvailable);
                        }
                        if (_ct.IsCancellationRequested)
                        {
                            _stream.Close();
                            _client.Close();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hiba a CamTCP-ben: " + ex.Message);
                    Images?.Invoke(new Bitmap("nosignal2.jpg"));
                }
            }, _cancelTokenSource.Token);
        }

        public void Stop()
        {
            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
            }
        }

        private byte[] RemoveNullElement(byte[] tomb)
        {
            int vag = -1;
            for (int i = tomb.Length - 1; i >= 0; i--)
            {
                if (tomb[i] != 0)
                {
                    vag = i;
                    break;
                }
            }
            if (vag > 0)
            {
                byte[] tomb2 = new byte[vag + 1];
                for (int i = 0; i < vag + 1; i++)
                {
                    tomb2[i] = tomb[i];
                }
                return tomb2;
            }
            return tomb;
        }
    }
}
