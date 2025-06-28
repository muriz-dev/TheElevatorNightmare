using System;
using UnityEngine;

// Struct yang sudah dimodifikasi untuk variasi
[System.Serializable]
public struct Sound
{
    public string name;
    public AudioClip[] clips; // Array untuk variasi

    [Range(0f, 1f)]
    public float volume;
    [Range(0.5f, 1.5f)]
    public float pitch;

    [Range(0f, 0.5f)]
    public float randomVolumeVariance;
    [Range(0f, 0.5f)]
    public float randomPitchVariance;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource loopingSource;

    [Header("Audio Clips")]
    [Tooltip("Daftar semua trek musik dalam game.")]
    public Sound[] musicSounds; // Menggunakan struct Sound yang baru
    [Tooltip("Daftar semua efek suara (SFX) dalam game.")]
    public Sound[] sfxSounds; // Menggunakan struct Sound yang baru

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {
        // Fungsi PlayMusic bisa dimodifikasi dengan cara yang sama jika perlu variasi,
        // tapi musik biasanya tidak butuh itu. Kita asumsikan musik hanya punya 1 klip.
        Sound s = Array.Find(musicSounds, sound => sound.name == name);
        if (s.clips == null || s.clips.Length == 0)
        {
            Debug.LogWarning("Music: " + name + " tidak ditemukan!");
            return;
        }
        musicSource.clip = s.clips[0]; // Ambil klip pertama untuk musik
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.loop = true;
        musicSource.Play();
    }

    // --- FUNGSI INI YANG DIPERBARUI ---
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s.clips == null || s.clips.Length == 0)
        {
            Debug.LogWarning("SFX: '" + name + "' tidak ditemukan atau tidak memiliki audio clips!");
            return;
        }

        AudioClip clipToPlay = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        
        sfxSource.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.randomPitchVariance / 2f, s.randomPitchVariance / 2f));
        float finalVolume = s.volume * (1f + UnityEngine.Random.Range(-s.randomVolumeVariance / 2f, s.randomVolumeVariance / 2f));

        sfxSource.PlayOneShot(clipToPlay, finalVolume);
    }

    // Fungsi looping juga bisa diadaptasi jika diperlukan
    public void PlayLoopingSound(string name)
    {
        if (loopingSource.isPlaying && loopingSource.clip != null && loopingSource.clip.name == name) {
            return;
        }
        
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s.clips == null || s.clips.Length == 0)
        {
            Debug.LogWarning("Looping SFX: " + name + " tidak ditemukan!");
            return;
        }
        
        loopingSource.clip = s.clips[0]; // Ambil klip pertama untuk looping SFX
        loopingSource.volume = s.volume;
        loopingSource.pitch = s.pitch;
        loopingSource.loop = true;
        loopingSource.Play();
    }

    public void StopLoopingSound()
    {
        if (loopingSource.isPlaying)
        {
            loopingSource.Stop();
        }
    }
}