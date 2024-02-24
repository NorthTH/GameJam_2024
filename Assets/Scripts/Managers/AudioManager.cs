using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGM { One = 0, Two = 1, Three = 2}
public enum SE {  }

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    AudioSource bgmAudioSource;
    [SerializeField]
    AudioSource seAudioSource;
    [SerializeField]
    AudioSource randomSeAudioSource;

    [SerializeField]
    AudioClip[] bgmAudioClips;
    [SerializeField]
    AudioClip[] seAudioClips;


    public void PlayBgm(BGM bgm)
    {
        bgmAudioSource.clip = bgmAudioClips[(int)bgm - 1];
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    public void PlaySe(SE se)
    {
        seAudioSource.clip = seAudioClips[(int)se - 1];
        seAudioSource.Play();
    }

    public void RandomSe(SE se)
    {
        seAudioSource.clip = seAudioClips[(int)se - 1];
        seAudioSource.Play();
    }
}
