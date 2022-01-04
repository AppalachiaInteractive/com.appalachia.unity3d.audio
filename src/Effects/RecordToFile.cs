using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using UnityEngine;

namespace Appalachia.Audio.Effects
{
    public sealed class RecordToFile : AppalachiaBehaviour<RecordToFile>
    {
        #region Fields and Autoproperties

        private readonly List<byte[]> _proc = new(8);
        private readonly object _locker = 0;
        private readonly Queue<byte[]> _freed = new(8);
        private readonly Queue<byte[]> _inuse = new(8);
        private BinaryWriter _writer;
        private bool _recording;

        #endregion

        #region Event Functions

        private void Update()
        {
            if (!DependenciesAreReady || !FullyInitialized)
            {
                return;
            }
            
            if (_recording)
            {
                lock (_locker)
                {
                    while (_inuse.Count > 0)
                    {
                        _proc.Add(_inuse.Dequeue());
                    }
                }

                foreach (var i in _proc)
                {
                    _writer.Write(i);
                }

                lock (_locker)
                {
                    foreach (var i in _proc)
                    {
                        _freed.Enqueue(i);
                    }
                }

                _proc.Clear();
            }
        }

        protected override async AppaTask WhenDestroyed()
        {
            await base.WhenDestroyed();
            
            StopRecording();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            lock (_locker)
            {
                if (_recording)
                {
                    var buf = _freed.Count > 0 ? _freed.Dequeue() : null;
                    if ((buf == null) || (buf.Length != (data.Length * sizeof(float))))
                    {
                        buf = new byte[data.Length * sizeof(float)];
                    }

                    Buffer.BlockCopy(data, 0, buf, 0, buf.Length);
                    _inuse.Enqueue(buf);
                }
            }
        }

        #endregion

        public void StartRecording(string path)
        {
            StopRecording();
            if (!path.EndsWith(".wav"))
            {
                path += ".wav";
            }

            lock (_locker)
            {
                _writer = new BinaryWriter(new FileStream(path, FileMode.Create));
                for (var i = 0; i < 44; ++i)
                {
                    _writer.Write((byte) 0x00);
                }

                _recording = true;
            }
        }

        public int StopRecording()
        {
            var len = 0;
            if (_recording)
            {
                lock (_locker)
                {
                    len = (int) _writer.BaseStream.Length;
                    _recording = false;

                    short format = 3;
                    short channels = 2;
                    var sampleRate = AudioSettings.outputSampleRate;
                    var byteRate = sampleRate * sizeof(float) * channels;
                    var blockAlign = (short) (sizeof(float) * channels);
                    short bitsPerSample = sizeof(float) * 8;
                    _writer.Seek(0, SeekOrigin.Begin);
                    _writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                    _writer.Write((int) (_writer.BaseStream.Length - 8));
                    _writer.Write(Encoding.ASCII.GetBytes("WAVE"));
                    _writer.Write(Encoding.ASCII.GetBytes("fmt "));
                    _writer.Write(16);
                    _writer.Write(format);
                    _writer.Write(channels);
                    _writer.Write(sampleRate);
                    _writer.Write(byteRate);
                    _writer.Write(blockAlign);
                    _writer.Write(bitsPerSample);
                    _writer.Write(Encoding.ASCII.GetBytes("data"));
                    _writer.Write((int) (_writer.BaseStream.Length - 44));
                    _writer.Close();
                }
            }

            return len;
        }
    }
}
