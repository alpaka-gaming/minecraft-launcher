using System;
using System.IO;
using System.Linq;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.SoundOut;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class MusicPlayer : IMusicPlayer
    {
        public PlaybackState Status => _soundOut?.PlaybackState ?? PlaybackState.Stopped;

        private IWaveSource _waveSource;
        private ISoundOut _soundOut;
        private Stream[] _streams;
        private int _currentTrack;

        public void Play()
        {
            _soundOut?.Play();
        }

        public void Play(Stream stream)
        {
            _waveSource = GetSoundSource(stream);
            _soundOut = GetSoundOut();
            _soundOut.Initialize(_waveSource);
            OnStarted(this, EventArgs.Empty);
        }

        public void Play(Stream[] stream)
        {
            if (stream != null && stream.Any())
            {
                _streams = stream;
                Play(stream.First());
            }
        }

        private void OnStopped(object sender, EventArgs e)
        {
            _soundOut?.Stop();
            Stopped?.Invoke(sender, e);
        }

        private void OnStarted(object sender, EventArgs e)
        {
            _soundOut?.Play();
            Started?.Invoke(sender, e);
        }

        private void OnPaused(object sender, EventArgs e)
        {
            _soundOut?.Pause();
            Paused?.Invoke(sender, e);
        }

        private void OnTrackChanged(object sender, EventArgs e)
        {
            Play(_streams[_currentTrack]);
            TrackChanged?.Invoke(sender, e);
        }

        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Paused;
        public event EventHandler<EventArgs> TrackChanged;

        private IWaveSource GetSoundSource(Stream stream)
        {
            return new DmoMp3Decoder(stream);
        }

        private ISoundOut GetSoundOut()
        {
            if (WasapiOut.IsSupportedOnCurrentPlatform)
                return new WasapiOut();
            else
                return new DirectSoundOut();
        }

        public void Stop()
        {
            _soundOut?.Stop();
            
            _soundOut?.Dispose();
            _soundOut = null;
            
            _waveSource?.Dispose();
            _waveSource = null;
            
            OnStopped(this, EventArgs.Empty);
        }

        public void Pause()
        {
            _soundOut?.Pause();
            OnPaused(this, EventArgs.Empty);
        }

        public void Next()
        {
            _currentTrack++;
            if (_currentTrack > _streams?.Length) _currentTrack = (int) _streams?.Length;
            OnTrackChanged(this, EventArgs.Empty);
        }

        public void Previous()
        {
            _currentTrack--;
            if (_currentTrack < 0) _currentTrack = 0;
            OnTrackChanged(this, EventArgs.Empty);
        }
    }
}