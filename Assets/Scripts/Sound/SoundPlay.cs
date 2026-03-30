using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{
    public  AudioClip[] audioClips;
    public  AudioSource AudioSource;
    public void PlaySound(int num)
    {
        AudioSource.clip = audioClips[num];
        AudioSource.Play();
    }
}
