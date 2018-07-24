using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace TalkClient
{
    class AudioManager
    {

        const int SAMPLERATE = 48000;

        System.Windows.Forms.Timer time = new System.Windows.Forms.Timer();
        public event EventHandler<byte[]> DataReceieved = delegate { };
        WaveInEvent WavIn;
        int _segframes;
        int bytessent = 0;
        int _bytesPerSegment;
        DateTime starttime;
        public AudioManager()
        {
            starttime = DateTime.Now;
            time = new System.Windows.Forms.Timer();
            time.Interval = 1000;
            time.Tick += Time_Tick;
            time.Start();

            WavIn = new WaveInEvent();
            WavIn.BufferMilliseconds = 50;
            WavIn.WaveFormat = new WaveFormat(SAMPLERATE, 16, 1);
            WavIn.DeviceNumber = -1;
            WavIn.DataAvailable += WavIn_DataAvailable;
            WavIn.StartRecording();
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            var timeDiff = DateTime.Now - starttime;
            var bytesPerSecond = bytessent / timeDiff.TotalSeconds;
            Debug.WriteLine("{0} Bps", bytesPerSecond);
        }

        byte[] _notEncodedBuffer = new byte[0];
        private void WavIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            DataReceieved(this, e.Buffer);

            /*
            byte[] soundBuffer = new byte[e.BytesRecorded + _notEncodedBuffer.Length];
            for (int i = 0; i < _notEncodedBuffer.Length; i++)
                soundBuffer[i] = _notEncodedBuffer[i];
            for (int i = 0; i < e.BytesRecorded; i++)
                soundBuffer[i + _notEncodedBuffer.Length] = e.Buffer[i];

            int byteCap = _bytesPerSegment;
            int segmentCount = (int)Math.Floor((decimal)soundBuffer.Length / byteCap);
            int segmentsEnd = segmentCount * byteCap;
            int notEncodedCount = soundBuffer.Length - segmentsEnd;
            _notEncodedBuffer = new byte[notEncodedCount];
            for (int i = 0; i < notEncodedCount; i++)
            {
                _notEncodedBuffer[i] = soundBuffer[segmentsEnd + i];
            }

            for (int i = 0; i < segmentCount; i++)
            {
                byte[] segment = new byte[byteCap];
                for (int j = 0; j < segment.Length; j++)
                    segment[j] = soundBuffer[(i * byteCap) + j];
                int len;
                byte[] buff = Enc.Encode(segment, segment.Length, out len);
                byte[] _out = new byte[len];
                Array.Copy(buff, 0, _out, 0, len);
                bytessent += len;

                
            }*/
        }
    }
}
