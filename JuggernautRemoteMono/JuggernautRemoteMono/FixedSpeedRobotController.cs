using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JuggernautControlAndroid
{
    public class FixedSpeedRobotController
    {
        public RobotController rc;
        public int LS, RS;
        Action<MemoryStream> ReceivedFrame;

        private int _L;

        public int L
        {
            get { return getSpeed(_L, LS); }
            set { _L = setSpeed(value, LS); rc.L = (byte)_L; }
        }

        private int _R;

        public int R
        {
            get { return getSpeed(_R, RS); }
            set { _R = setSpeed(value, RS); rc.R = (byte)_R; }
        }

        public int setSpeed(int i, int s)
        {
            return 90 + (int)((i - 90) * (s / 90.0));
        }

        public int getSpeed(int i, int s)
        {
            return 90 + (int)((i - 90) * (90.0 / s));
        }

        public FixedSpeedRobotController(Action<MemoryStream> ReceivedFrame, int LS, int RS)
        {
            this.LS = LS;
            this.RS = RS;
            this.ReceivedFrame = ReceivedFrame;
        }

        public void start()
        {
            rc = new RobotController();
            rc.receivedFrame = ReceivedFrame;
            rc.startUdpServer();
           // rc.startTcpServer();
        }
    }
}
