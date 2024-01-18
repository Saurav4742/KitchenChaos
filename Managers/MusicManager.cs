using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour{

    public static MusicManager Instance {get;private set;}

    private AudioSource audioSource;
    private float volume = .3f;
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";


    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        float defaultVolume = .3f;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME,defaultVolume);
        audioSource.volume = volume;
    }

    public void ChangeVolume() {
        volume += .1f;
        if(volume > 1f){
            volume=0f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME,volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return volume;
    }
}
