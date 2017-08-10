using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using RawLauncher.Framework.NAudio.Wave.MmeInterop;
using RawLauncher.Framework.NAudio.Wave.WaveFormats;
using RawLauncher.Framework.NAudio.Wave.WaveStreams;

namespace RawLauncher.Framework.NAudio.Wave.WaveOutputs 
{
    public class WaveOut : IWavePlayer, IWavePosition
    {
        private IntPtr _hWaveOut;
        private WaveOutBuffer[] _buffers;
        private IWaveProvider _waveStream;
        private volatile PlaybackState _playbackState;
        private readonly WaveInterop.WaveCallback _callback;
        private float _volume = 1;
        private readonly WaveCallbackInfo _callbackInfo;
        private readonly object _waveOutLock;
        private int _queuedBuffers;
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Indicates playback has stopped automatically
        /// </summary>
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        /// <summary>
        /// Gets or sets the desired latency in milliseconds
        /// Should be set before a call to Init
        /// </summary>
        private int DesiredLatency { get; }

        /// <summary>
        /// Gets or sets the number of buffers used
        /// Should be set before a call to Init
        /// </summary>
        private int NumberOfBuffers { get; }

        /// <summary>
        /// Gets or sets the device number
        /// Should be set before a call to Init
        /// This must be between -1 and <see>DeviceCount</see> - 1.
        /// -1 means stick to default device even default device is changed
        /// </summary>
        private int DeviceNumber { get; } = -1;

        public WaveOut()
            : this(SynchronizationContext.Current == null ? WaveCallbackInfo.FunctionCallback() : WaveCallbackInfo.NewWindow())
        {
        }


        /// <summary>
        /// Opens a WaveOut device
        /// </summary>
        private WaveOut(WaveCallbackInfo callbackInfo)
        {
            _syncContext = SynchronizationContext.Current;
            // set default values up
            DeviceNumber = 0;
            DesiredLatency = 300;
            NumberOfBuffers = 2;

            _callback = Callback;
            _waveOutLock = new object();
            _callbackInfo = callbackInfo;
            callbackInfo.Connect(_callback);
        }

        public void Init(IWaveProvider waveProvider)
        {
            _waveStream = waveProvider;
            int bufferSize = waveProvider.WaveFormat.ConvertLatencyToByteSize((DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers);            

            MmResult result;
            lock (_waveOutLock)
            {
                result = _callbackInfo.WaveOutOpen(out _hWaveOut, DeviceNumber, _waveStream.WaveFormat, _callback);
            }
            MmException.Try(result, "waveOutOpen");

            _buffers = new WaveOutBuffer[NumberOfBuffers];
            _playbackState = PlaybackState.Stopped;
            for (int n = 0; n < NumberOfBuffers; n++)
            {
                _buffers[n] = new WaveOutBuffer(_hWaveOut, bufferSize, _waveStream, _waveOutLock);
            }
        }

        public void Play()
        {
            if (_playbackState == PlaybackState.Stopped)
            {
                _playbackState = PlaybackState.Playing;
                Debug.Assert(_queuedBuffers == 0, "Buffers already queued on play");
                EnqueueBuffers();
            }
            else if (_playbackState == PlaybackState.Paused)
            {
                EnqueueBuffers();
                Resume();
                _playbackState = PlaybackState.Playing;
            }
        }

        private void EnqueueBuffers()
        {
            for (int n = 0; n < NumberOfBuffers; n++)
            {
                if (!_buffers[n].InQueue)
                {
                    if (_buffers[n].OnDone())
                    {
                        Interlocked.Increment(ref _queuedBuffers);
                    }
                    else
                    {
                        _playbackState = PlaybackState.Stopped;
                        break;
                    }
                }
            }
        }

        public void Pause()
        {
            if (_playbackState == PlaybackState.Playing)
            {
                MmResult result;
                _playbackState = PlaybackState.Paused; // set this here, to avoid a deadlock with some drivers
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutPause(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutPause");
                }
            }
        }

        private void Resume()
        {
            if (_playbackState != PlaybackState.Paused)
                return;
            MmResult result;
            lock (_waveOutLock)
            {
                result = WaveInterop.waveOutRestart(_hWaveOut);
            }
            if (result != MmResult.NoError)
            {
                throw new MmException(result, "waveOutRestart");
            }
            _playbackState = PlaybackState.Playing;
        }

        public void Stop()
        {
            if (_playbackState != PlaybackState.Stopped)
            {

                // in the call to waveOutReset with function callbacks
                // some drivers will block here until OnDone is called
                // for every buffer
                _playbackState = PlaybackState.Stopped; // set this here to avoid a problem with some drivers whereby 
                MmResult result;
                lock (_waveOutLock)
                {
                    result = WaveInterop.waveOutReset(_hWaveOut);
                }
                if (result != MmResult.NoError)
                {
                    throw new MmException(result, "waveOutReset");
                }

                // with function callbacks, waveOutReset will call OnDone,
                // and so PlaybackStopped must not be raised from the handler
                // we know playback has definitely stopped now, so raise callback
                if (_callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback)
                {
                    RaisePlaybackStoppedEvent(null);
                }
            }
        }

        public long GetPosition()
        {
            lock (_waveOutLock)
            {
                MmTime mmTime = new MmTime {wType = MmTime.TIME_BYTES};
                // request results in bytes, TODO: perhaps make this a little more flexible and support the other types?
                MmException.Try(WaveInterop.waveOutGetPosition(_hWaveOut, out mmTime, Marshal.SizeOf(mmTime)), "waveOutGetPosition");

                if (mmTime.wType != MmTime.TIME_BYTES)
                    throw new Exception(
                        $"waveOutGetPosition: wType -> Expected {MmTime.TIME_BYTES}, Received {mmTime.wType}");

                return mmTime.cb;
            }
        }

        public WaveFormat OutputWaveFormat => _waveStream.WaveFormat;

        public PlaybackState PlaybackState => _playbackState;

        public float Volume
        {
            get => _volume;
            set 
            {
                SetWaveOutVolume(value, _hWaveOut, _waveOutLock);
                _volume = value;
            }
        }

        internal static void SetWaveOutVolume(float value, IntPtr hWaveOut, object lockObject)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Volume must be between 0.0 and 1.0");
            if (value > 1) throw new ArgumentOutOfRangeException(nameof(value), "Volume must be between 0.0 and 1.0");
            float left = value;
            float right = value;

            int stereoVolume = (int) (left*0xFFFF) + ((int) (right*0xFFFF) << 16);
            MmResult result;
            lock (lockObject)
            {
                result = WaveInterop.waveOutSetVolume(hWaveOut, stereoVolume);
            }
            MmException.Try(result, "waveOutSetVolume");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            Stop();

            if (disposing)
            {
                if (_buffers != null)
                {
                    foreach (var t in _buffers)
                        t?.Dispose();
                    _buffers = null;
                }
            }

            lock (_waveOutLock)
            {
                WaveInterop.waveOutClose(_hWaveOut);
            }
            if (disposing)
            {
                _callbackInfo.Disconnect();
            }
        }

        ~WaveOut()
        {
            Dispose(false);
        }



        // made non-static so that playing can be stopped here
        private void Callback(IntPtr hWaveOut, WaveInterop.WaveMessage uMsg, IntPtr dwInstance, WaveHeader wavhdr, IntPtr dwReserved)
        {
            if (uMsg == WaveInterop.WaveMessage.WaveOutDone)
            {
                GCHandle hBuffer = (GCHandle)wavhdr.userData;
                WaveOutBuffer buffer = (WaveOutBuffer)hBuffer.Target;
                Interlocked.Decrement(ref _queuedBuffers);
                Exception exception = null;
                // check that we're not here through pressing stop
                if (PlaybackState == PlaybackState.Playing)
                {
                    // to avoid deadlocks in Function callback mode,
                    // we lock round this whole thing, which will include the
                    // reading from the stream.
                    // this protects us from calling waveOutReset on another 
                    // thread while a WaveOutWrite is in progress
                    lock (_waveOutLock) 
                    {
                        try
                        {
                            if (buffer.OnDone())
                            {
                                Interlocked.Increment(ref _queuedBuffers);
                            }
                        }
                        catch (Exception e)
                        {
                            // one likely cause is soundcard being unplugged
                            exception = e;
                        }
                    }
                }
                if (_queuedBuffers == 0)
                {
                    if (_callbackInfo.Strategy == WaveCallbackStrategy.FunctionCallback && _playbackState == PlaybackState.Stopped)
                    {
                        // the user has pressed stop
                        // DO NOT raise the playback stopped event from here
                        // since on the main thread we are still in the waveOutReset function
                        // Playback stopped will be raised elsewhere
                    }
                    else
                    {
                        _playbackState = PlaybackState.Stopped; // set explicitly for when we reach the end of the audio
                        RaisePlaybackStoppedEvent(exception);
                    }
                }
            }
        }

        private void RaisePlaybackStoppedEvent(Exception e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (_syncContext == null)
                    handler(this, new StoppedEventArgs());
                else
                    _syncContext.Post(state => handler(this, new StoppedEventArgs()), null);
            }
        }
    }
}
