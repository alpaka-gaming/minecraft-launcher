using System;
using System.IO;
using CSCore.SoundOut;

namespace Infrastructure.Interfaces
{
    public interface IMusicPlayer
    {
        PlaybackState Status { get; }
        void Play(Stream stream);
        void Play(Stream[] stream);

        void Play();
        void Stop();
        void Pause();
        void Next();
        void Previous();
        
        event EventHandler<EventArgs> Stopped;
        event EventHandler<EventArgs> Paused;
        event EventHandler<EventArgs> Started;
        event EventHandler<EventArgs> TrackChanged;
    }
}