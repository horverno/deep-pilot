using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Windows.Gaming.Input;

namespace DeepPilotCsharp.CarController
{
    public class InputController
    {
        private int _currentspeed = 1024;
        private const int MAXBACKWARD = 1023;
        private const int MAXFORWARD = 2047;
        private const int MOTORSTOPHATRA = 0;
        private const int MOTORSTOPELORE = 1024;
        private const double WHEELSTREERING = 511.5;
        //(hátra)0-1023 || (előre)1024-2047 || 0,1024 stop
        public delegate void GetMessage(MessageFromComputerToRaspberry message);
        public event GetMessage MessageTo;

        private bool _right = false;
        private bool _left = false;
        private bool _forward = false;
        private bool _backward = false;
        private double _wheelsteering = WHEELSTREERING; ////0-1023

        private DispatcherTimer _keyboardreadtimer;
        private DispatcherTimer _wheelreadtimer;
        private RacingWheel _controller;

        private UDPSender _udPSender = null;
        private bool IsConnected = false; //Socket dispose

        public InputController()
        {
            //RacingWheel connection
            RacingWheel.RacingWheelAdded += RacingWheel_RacingWheelAdded;
            RacingWheel.RacingWheelRemoved += RacingWheel_RacingWheelRemoved;

            //Setup timer for wheel read
            _wheelreadtimer = new DispatcherTimer();
            _wheelreadtimer.Tick += _wheelreadtimer_Tick; ;
            _wheelreadtimer.Interval = TimeSpan.FromMilliseconds(100);

            //Setup timer for keyboard read
            _keyboardreadtimer = new DispatcherTimer();
            _keyboardreadtimer.Tick += _keyboardreadtimer_Tick; ;
            _keyboardreadtimer.Interval = TimeSpan.FromMilliseconds(100);
        }

        public void ConnectToServer(string IPAddress, int port)
        {
            try
            {
                IsConnected = false;
                if (_udPSender == null)//First connect
                {
                    _udPSender = new UDPSender(IPAddress, port);
                }
                else //Reconnect
                {
                    bool wheel = _wheelreadtimer.IsEnabled;
                    bool keyboard = _keyboardreadtimer.IsEnabled;
                    if (wheel)
                    {
                        _wheelreadtimer.Stop();
                    }

                    if (keyboard)
                    {
                        _keyboardreadtimer.Stop();
                    }

                    _udPSender.Close();
                    _udPSender = new UDPSender(IPAddress, port);
                    if (wheel)
                    {
                        _wheelreadtimer.Start();
                    }

                    if (keyboard)
                    {
                        _keyboardreadtimer.Start();
                    }
                }
                IsConnected = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                MessageBox.Show("InputController " + ex.Message);
            }
        }

        public void Stop()
        {
            _wheelreadtimer.Stop();
            _keyboardreadtimer.Stop();
            if (IsConnected)
            {
                _udPSender.Close();
                IsConnected = false;
                Console.WriteLine("Disconnected...");
            }
        }

        #region racingwheel
        private void _wheelreadtimer_Tick(object sender, EventArgs e)
        {
            if (RacingWheel.RacingWheels.Count > 0)
            {
                _controller = RacingWheel.RacingWheels.First();
                RacingWheelReading Reading = _controller.GetCurrentReading();
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry();
                switch (Reading.Buttons)
                {
                    case RacingWheelButtons.None:
                        break;
                    case RacingWheelButtons.PreviousGear:
                        break;
                    case RacingWheelButtons.NextGear:
                        break;
                    case RacingWheelButtons.DPadUp:
                        break;
                    case RacingWheelButtons.DPadDown:
                        break;
                    case RacingWheelButtons.DPadLeft:
                        break;
                    case RacingWheelButtons.DPadRight:
                        break;
                    case RacingWheelButtons.Button1:
                        //Korbány jobb legfelső gombja
                        break;
                    case RacingWheelButtons.Button2:
                        //Korbány jobb közép gombja
                        break;
                    case RacingWheelButtons.Button3:
                        //Korbány jobb legalsó gombja
                        break;
                    case RacingWheelButtons.Button4:
                        //Korbány bal legfelső gombja
                        break;
                    case RacingWheelButtons.Button5:
                        //Korbány bal közép gombja
                        break;
                    case RacingWheelButtons.Button6:
                        //Korbány bal legalsó gombja
                        break;
                    case RacingWheelButtons.Button7:
                        break;
                    case RacingWheelButtons.Button8:
                        break;
                    case RacingWheelButtons.Button9:
                        break;
                    case RacingWheelButtons.Button10:
                        break;
                    case RacingWheelButtons.Button11:
                        break;
                    case RacingWheelButtons.Button12:
                        break;
                    case RacingWheelButtons.Button13:
                        break;
                    case RacingWheelButtons.Button14:
                        break;
                    case RacingWheelButtons.Button15:
                        break;
                    case RacingWheelButtons.Button16:
                        break;
                    default:
                        break;
                }
                double wheelPosition = Reading.Wheel;
                Message.SetAngleReferenceSteering = (int)((wheelPosition + 1) * 511.5); //0-1023
                double gasPedal = Reading.Throttle;
                Message.SetSpeedSignalReferenceDrive = (int)(gasPedal * 2047); //(hátra)0-1023 || (előre)1024-2047
                if (_udPSender != null && IsConnected)
                {
                    _udPSender.SendText(Message.Serialize());
                    MessageTo?.Invoke(Message);
                }
            }
        }

        private void RacingWheel_RacingWheelRemoved(object sender, RacingWheel e)
        {
            Console.WriteLine("RacingWheel disconnect!");
        }

        private void RacingWheel_RacingWheelAdded(object sender, RacingWheel e)
        {
            Console.WriteLine("RacingWheel connect!");
        }

        public void ChangeRaicingWheelReadingInterval(TimeSpan ts)
        {
            _wheelreadtimer.Interval = ts;
        }

        public void DisableRaicingWheel()
        {
            _wheelreadtimer.Stop();

        }

        public void AbleRaicingWheel()
        {
            if (IsConnected)
            {
                _wheelreadtimer.Start();
            }
        }

        #endregion
        #region keyboard
        private void _keyboardreadtimer_Tick(object sender, EventArgs e)
        {
            if (_forward || _backward || _right || _left)
            {
                if (_currentspeed >= MOTORSTOPELORE)
                {
                    if (_forward)
                    {
                        _currentspeed += 100;
                        if (_currentspeed > MAXFORWARD)
                        {
                            _currentspeed = MAXFORWARD;
                        }
                    }
                    if (_backward)
                    {
                        _currentspeed -= 100;
                        if (_currentspeed < MOTORSTOPELORE)
                        {
                            _currentspeed = MOTORSTOPELORE - _currentspeed;
                        }
                    }
                }
                else
                {
                    if (_backward)
                    {
                        _currentspeed += 100;
                        if (_currentspeed > MAXBACKWARD)
                        {
                            _currentspeed = MAXBACKWARD;
                        }
                    }
                    if (_forward)
                    {
                        _currentspeed -= 100;
                        if (_currentspeed < MOTORSTOPHATRA)
                        {
                            _currentspeed = MOTORSTOPELORE + Math.Abs(_currentspeed);
                        }
                    }
                }

                if (_right)
                {
                    _wheelsteering += 51.5;
                    if (_wheelsteering > 1023)
                    {
                        _wheelsteering = 1023;
                    }
                }
                if (_left)
                {
                    _wheelsteering -= 51.5;
                    if (_wheelsteering < 0)
                    {
                        _wheelsteering = 0;
                    }
                }
                Console.WriteLine(_currentspeed + " | " + _wheelsteering);
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry()
                {
                    SetAngleReferenceSteering = (int)_wheelsteering,
                    SetSpeedSignalReferenceDrive = _currentspeed
                };
                if (_udPSender != null && IsConnected)
                {
                    _udPSender.SendText(Message.Serialize());
                    MessageTo?.Invoke(Message);
                }
            }
        }

        public void ChangeKeyboardReadingInterval(TimeSpan ts)
        {
            _keyboardreadtimer.Interval = ts;
        }

        public void DisableKeyboard()
        {
            _keyboardreadtimer.Stop();
        }

        public void AbleKeyboard()
        {
            if (IsConnected)
            {
                _keyboardreadtimer.Start();
            }
        }

        public void SetForward(bool forward)
        {
            _forward = forward;
            if (!_forward && _keyboardreadtimer.IsEnabled)
            {
                SetWheelAndSpeedToDown();
            }
        }

        public void SetBackward(bool backward)
        {
            _backward = backward;
            if (!_backward && _keyboardreadtimer.IsEnabled)
            {
                SetWheelAndSpeedToDown();
            }
        }

        public void SetRight(bool right)
        {
            _right = right;
            if (!_right && _keyboardreadtimer.IsEnabled)
            {
                SetWheelAndSpeedToDown();
            }
        }

        public void SetLeft(bool left)
        {
            _left = left;
            if (!_left && _keyboardreadtimer.IsEnabled)
            {
                SetWheelAndSpeedToDown();
            }
        }

        private void SetWheelAndSpeedToDown()
        {
            if (!_forward && !_backward)
            {
                _currentspeed = MOTORSTOPELORE;
            }
            if (!_right && !_left)
            {
                _wheelsteering = WHEELSTREERING;
            }

            if (!_forward && !_backward && !_right && !_left)
            {
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry()
                {
                    SetAngleReferenceSteering = (int)WHEELSTREERING,
                    SetSpeedSignalReferenceDrive = MOTORSTOPELORE
                };
                if (_udPSender != null && IsConnected)
                {
                    _udPSender.SendText(Message.Serialize());
                    MessageTo?.Invoke(Message);
                    Console.WriteLine(Message.Serialize());
                }
            }
        }
        #endregion
        #region anotherMsg
        public void SendMsgDirect(MessageFromComputerToRaspberry Message)
        {
            if (_udPSender != null && IsConnected)
            {
                _udPSender.SendText(Message.Serialize());
                MessageTo?.Invoke(Message);
            }
        }

        public void GetCam()
        {
            if (_udPSender != null && IsConnected)
            {
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry()
                {
                    SendCamera1 = true,
                    SendCamera2 = true
                };
                _udPSender.SendText(Message.Serialize());
                MessageTo?.Invoke(Message);
                Console.WriteLine("Cam request send");
            }
        }

        public void GetCam1()
        {
            if (_udPSender != null && IsConnected)
            {
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry()
                {
                    SendCamera1 = true
                };
                _udPSender.SendText(Message.Serialize());
                MessageTo?.Invoke(Message);
                Console.WriteLine("Cam1 request send");
            }
        }

        public void GetCam2()
        {
            if (_udPSender != null && IsConnected)
            {
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry()
                {
                    SendCamera2 = true
                };
                _udPSender.SendText(Message.Serialize());
                MessageTo?.Invoke(Message);
                Console.WriteLine("Cam2 request send");
            }
        }

        public void GetSend()
        {
            if (_udPSender != null && IsConnected)
            {
                MessageFromComputerToRaspberry Message = new MessageFromComputerToRaspberry()
                {
                    SendAll = true,
                    SendImportant = true,
                    SendCamera1 = true,
                    SendCamera2 = true
                };
                _udPSender.SendText(Message.Serialize());
                MessageTo?.Invoke(Message);
                Console.WriteLine("Send All, Send Important, Send Camera...");
            }
        }
        #endregion
    }
}
