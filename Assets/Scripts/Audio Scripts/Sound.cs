using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    [Range(0f, 1f)]
    public float spatialBlend;

    [Range(1f, 10f)]
    public float maxDistance;

    [Range(0f, 1f)]
    public float minDistance;

    [HideInInspector]
    public AudioSource source;


}
