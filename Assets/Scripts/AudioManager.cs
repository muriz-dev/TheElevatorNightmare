using System; // Diperlukan untuk Array.Find
using UnityEngine;

// Struct ini memungkinkan kita untuk mengasosiasikan nama dengan klip audio di Inspector.
[System.Serializable]
public struct Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    // Pola Singleton untuk akses mudah dari skrip lain
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource loopingSource; // Satu source untuk semua SFX yang berulang

    [Header("Audio Clips")]
    [Tooltip("Daftar semua trek musik dalam game.")]
    public Sound[] musicSounds;
    [Tooltip("Daftar semua efek suara (SFX) dalam game.")]
    public Sound[] sfxSounds;

    void Awake()
    {
        // Pengaturan Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Agar AudioManager tidak hancur saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    // Fungsi untuk memainkan musik berdasarkan nama
    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(musicSounds, sound => sound.name == name);

        if (sound.clip == null)
        {
            Debug.LogWarning("Music: " + name + " tidak ditemukan!");
            return;
        }

        musicSource.clip = sound.clip;
        musicSource.loop = true; // Musik biasanya berulang
        musicSource.Play();
    }

    // Fungsi untuk memainkan SFX sekali jalan (one-shot) berdasarkan nama
    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(sfxSounds, sound => sound.name == name);

        if (sound.clip == null)
        {
            Debug.LogWarning("SFX: " + name + " tidak ditemukan!");
            return;
        }

        sfxSource.PlayOneShot(sound.clip);
    }

    // Fungsi untuk memulai SFX yang berulang (looping) berdasarkan nama
    public void PlayLoopingSound(string name)
    {
        // Jika suara yang sama sudah diputar, jangan lakukan apa-apa
        if (loopingSource.isPlaying && loopingSource.clip.name == name) {
            return;
        }
        
        Sound sound = Array.Find(sfxSounds, sound => sound.name == name);

        if (sound.clip == null)
        {
            Debug.LogWarning("Looping SFX: " + name + " tidak ditemukan!");
            return;
        }
        
        loopingSource.clip = sound.clip;
        loopingSource.loop = true;
        loopingSource.Play();
    }

    // Fungsi untuk menghentikan SFX yang sedang berulang
    public void StopLoopingSound()
    {
        if (loopingSource.isPlaying)
        {
            loopingSource.Stop();
        }
    }
}