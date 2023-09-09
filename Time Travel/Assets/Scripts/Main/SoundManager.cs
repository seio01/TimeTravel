using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] clips;
    public int soundIndex;

    public bool soundOn = true;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(gameObject); 
    }

    public void SoundPlayer(string name)
    {
        switch (name)
        {
            case "Button":
                sfxPlayer[soundIndex].clip = clips[0];
                break;
            case "ItemPick":
                sfxPlayer[soundIndex].clip = clips[1];
                break;
            case "FlipPage":
                sfxPlayer[soundIndex].clip = clips[2];
                break;
            case "Warning":
                sfxPlayer[soundIndex].clip = clips[3];
                break;
            case "ShowPanel":
                sfxPlayer[soundIndex].clip = clips[4];
                break;
            case "RollDice":
                sfxPlayer[soundIndex].clip = clips[5];
                break;
            case "Correct":
                sfxPlayer[soundIndex].clip = clips[6];
                break;
            case "Wrong":
                sfxPlayer[soundIndex].clip = clips[7];
                break;
            case "ChangeClothes":
                sfxPlayer[soundIndex].clip = clips[8];
                break;
            case "10Timer":
                sfxPlayer[soundIndex].clip = clips[9];
                break;
            case "5Timer":
                sfxPlayer[soundIndex].clip = clips[10];
                break;
            case "Portal":
                sfxPlayer[soundIndex].clip = clips[11];
                break;
            case "Finish":
                sfxPlayer[soundIndex].clip = clips[12];
                break;
            case "ShowPanel1":
                sfxPlayer[soundIndex].clip = clips[13];
                break;
            case "ShowPanel2":
                sfxPlayer[soundIndex].clip = clips[14];
                break;
            case "Button1":
                sfxPlayer[soundIndex].clip = clips[15];
                break;

        }
        sfxPlayer[soundIndex].Play();
        soundIndex = (soundIndex + 1) % sfxPlayer.Length;
    }
    
    public void SoundPlayerStop()
    {
        sfxPlayer[soundIndex].Stop();
    }

    public void ChangeVolumeOfBGMPlayer()
    {
        soundOn = !soundOn;
        if(!soundOn)
            bgmPlayer.volume = 0;
        else
            bgmPlayer.volume = 0.1f;
    }
}
