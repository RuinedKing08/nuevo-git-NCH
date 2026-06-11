using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("Source")]
    public AudioClip clip;
    public AudioResource randomContainer;
    public bool useRandomContainer;

    

    [Header("Settings")]
    public bool loop;
    public bool is3D;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Header("Random Variation")]
    public bool randomizePitch = false;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    public bool randomizeVolume = false;
    public Vector2 volumeRange = new Vector2(0.9f, 1f);

    [Header("Mixer (optional)")]
    public AudioMixerGroup mixerGroup;
}