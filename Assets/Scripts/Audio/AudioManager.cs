using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Pool")]
    [SerializeField] private int initialPoolSize = 10;
    private List<AudioSource> sources = new List<AudioSource>();

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

    
    public void Play(AudioData data)
    {
        Play(data, Vector3.zero);
    }

    
    public void Play(AudioData data, Vector3 position)
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
    }

    public void StopAll()
    {
        foreach (var source in sources)
        {
            source.Stop();
        }
    }
}
