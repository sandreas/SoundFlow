using SoundFlow.Abstracts;
using SoundFlow.Enums;
using SoundFlow.Interfaces;

namespace SoundFlow.Components;

/// <summary>
/// An abstract sound player that plays audio from a data provider.
/// </summary>
public abstract class SoundPlayerBase(ISoundDataProvider dataProvider): SoundComponent, ISoundPlayer
{
    /// <summary>Playback speed (protected)</summary>
    protected float _playbackSpeed = 1.0f;
    
    /// <summary>Sample position (protected)</summary>
    protected int _samplePosition = 0;
    
    /// <summary>Loop start samples (protected)</summary>
    protected int _loopStartSamples;
    
    /// <summary>Loop end samples (protected)</summary>
    protected int _loopEndSamples = -1;
    
    protected readonly ISoundDataProvider _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));

    
    /// <inheritdoc />
    public PlaybackState State { get; protected set; }
    
    /// <inheritdoc />
    public bool IsLooping { get; set; }

    /// <inheritdoc />
    public float PlaybackSpeed
    {
        get => _playbackSpeed;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Playback speed must be greater than zero.");
            _playbackSpeed = value;
        }
    }
    
    /// <inheritdoc />
    public float Time => (float)_samplePosition / AudioEngine.Channels / AudioEngine.Instance.SampleRate / PlaybackSpeed;

    /// <inheritdoc />
    public float Duration => (float)_dataProvider.Length / AudioEngine.Channels / AudioEngine.Instance.SampleRate / PlaybackSpeed;

    /// <inheritdoc />
    public int LoopStartSamples => _loopStartSamples;
    
    /// <inheritdoc />
    public int LoopEndSamples => _loopEndSamples;

    /// <inheritdoc />
    public float LoopStartSeconds => (float)_loopStartSamples / AudioEngine.Channels / AudioEngine.Instance.SampleRate;

    /// <inheritdoc />
    public float LoopEndSeconds => _loopEndSamples == -1 ? -1 : (float)_loopEndSamples / AudioEngine.Channels / AudioEngine.Instance.SampleRate;


    /// <inheritdoc />
    public void Play()
    {
        Enabled = true;
        State = PlaybackState.Playing;
    }
    
    /// <inheritdoc />
    public void Pause()
    {
        Enabled = false;
        State = PlaybackState.Paused;
    }
    
    /// <inheritdoc cref="ISoundPlayer"/>
    public void Stop()
    {
        Pause();
        Seek(0);
    }
    
    /// <inheritdoc cref="ISoundPlayer"/>
    public abstract void Seek(float time);

    /// <inheritdoc cref="ISoundPlayer"/>
    public abstract void Seek(int sampleOffset);

    /// <inheritdoc cref="ISoundPlayer"/>
    public void Seek(TimeSpan offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
    {
        var seekOffset = (float)offset.TotalMilliseconds / 1000;
        switch (seekOrigin)
        {
            case SeekOrigin.Current:
                Seek(Time + seekOffset);
                break;
            case SeekOrigin.End:
                Seek(Duration + seekOffset);
                break;
            case SeekOrigin.Begin:
            default:
                Seek(seekOffset);
                break;
        }
    }

    /// <inheritdoc cref="ISoundPlayer"/>
    public abstract void SetLoopPoints(float startTime, float? endTime);

    /// <inheritdoc cref="ISoundPlayer"/>
    public abstract void SetLoopPoints(int startSample, int endSample = -1);
}