using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace TalkClient.Debugging
{
    class TempPlayback
    {
        WaveOutEvent WavOut;
        BufferedWaveProvider Buff;
        string owner;

        public TempPlayback(string ownedby)
        {
            owner = ownedby;
            Buff = new BufferedWaveProvider(new WaveFormat(48000, 1));
            WavOut = new WaveOutEvent();
            WavOut.DeviceNumber = -1;
            //WavOut.DesiredLatency = 25;
            WavOut.Init(Buff);
            WavOut.Play();
        }

        public string Owner { get => owner;}

        public void AddBytes(byte[] data)
        {
                Buff.AddSamples(data,0, data.Length);
        }

        public void Exit()
        {
            WavOut.Stop();
            WavOut.Dispose();
            Buff = null;
        }
    }
}
