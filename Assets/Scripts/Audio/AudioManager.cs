using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Pool")]
    [SerializeField] private int initialPoolSize = 10;
    private List<AudioSource> sources = new List<AudioSource>();
    
    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    
    private AudioSource currentLoopedSound;
    
    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreatePool();
    }

    void CreatePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewSource();
        }
    }

    AudioSource CreateNewSource()
    {
        GameObject obj = new GameObject("AudioSource");
        obj.transform.parent = transform;

        AudioSource source = obj.AddComponent<AudioSource>();
        sources.Add(source);

        return source;
    }

    AudioSource GetAvailableSource()
    {
        foreach (var source in sources)
        {
            if (!source.isPlaying)
                return source;
        }

        return CreateNewSource();
    }

    
    public AudioSource Play(AudioData data)
    {
        return Play(data, Vector3.zero);
    }

    
    public AudioSource Play(AudioData data, Vector3 position)
    {
        AudioSource source = GetAvailableSource();
        
        if (data.is3D)
        {
            source.transform.position = position;
            source.spatialBlend = 1f;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        
        if (data.useRandomContainer && data.randomContainer != null)
        {
            source.resource = data.randomContainer;
        }
        else
        {
            source.clip = data.clip;
        }

        
        source.loop = data.loop;
        source.volume = data.volume;
        source.pitch = data.pitch;

        if (data.randomizePitch)
        {
            source.pitch = Random.Range(data.pitchRange.x, data.pitchRange.y);
        }

        if (data.randomizeVolume)
        {
            source.volume = Random.Range(data.volumeRange.x, data.volumeRange.y);
        }

        
        if (data.mixerGroup != null)
        {
            source.outputAudioMixerGroup = data.mixerGroup;
        }

        source.Play();
        return source;
    }

    public AudioSource PlayLooped(AudioData data)
    {
        return PlayLooped(data, Vector3.zero);
    }

    public AudioSource PlayLooped(AudioData data, Vector3 position)
    {
        
        if (currentLoopedSound != null && currentLoopedSound.isPlaying)
            currentLoopedSound.Stop();

        
        currentLoopedSound = Play(data, position);
        return currentLoopedSound;
    }

    public void StopLoopedSound()
    {
        if (currentLoopedSound != null)
            currentLoopedSound.Stop();
    }

    public AudioSource GetCurrentLoopedSound()
    {
        return currentLoopedSound;
    }

    public void StopAll()
    {
        foreach (var source in sources)
        {
            source.Stop();
        }
    }

    public void Stop(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }

    public void Stop(AudioData data)
    {
        if (data == null) return;

        AudioClip clipToFind = null;

        if (!data.useRandomContainer && data.clip != null)
        {
            clipToFind = data.clip;
        }

        if (clipToFind == null) return;

        foreach (var source in sources)
        {
            if (source.clip == clipToFind && source.isPlaying)
            {
                source.Stop();
                return;
            }
        }
    }
    
    // ===== VOLUME CONTROL =====
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (masterGroup != null)
            masterGroup.audioMixer.SetFloat("Master", VolumeToDecibels(masterVolume));
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxGroup != null)
            sfxGroup.audioMixer.SetFloat("SFX", VolumeToDecibels(sfxVolume));
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicGroup != null)
            musicGroup.audioMixer.SetFloat("Music", VolumeToDecibels(musicVolume));
    }
    
    public float GetMasterVolume() => masterVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetMusicVolume() => musicVolume;
    
    private float VolumeToDecibels(float volume)
    {
        if (volume <= 0f) return -80f;
        return Mathf.Log10(volume) * 20f;
    }
}
