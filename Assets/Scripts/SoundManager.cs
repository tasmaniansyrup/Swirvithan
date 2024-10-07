using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        if(PlayerPrefs.HasKey("musicVolume"))
        {
            Load();
        }
    }

    public void ChangeVolumeWithSlider()
    {
        AudioListener.volume = volumeSlider.value;
    }

    private void Load()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume");
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    public void AdjustSlider()
    {
        volumeSlider.value = AudioListener.volume;
    }
}
