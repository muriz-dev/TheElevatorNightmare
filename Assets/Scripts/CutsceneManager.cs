using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct CutsceneSoundTag
{
    [Tooltip("Tag yang akan Anda tulis di dalam teks, misal: 'gedebuk'")]
    public string tag;
    [Tooltip("Nama suara yang terdaftar di dalam sfxSounds pada AudioManager")]
    public string soundName;
}

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    [TextArea(3, 10)]
    public string[] sentences;
    public string nextSceneName;

    public float typingSpeed = 0.05f;
    public float sentenceDelay = 1.5f;

    [Header("Audio Settings (via AudioManager)")]
    [Tooltip("Nama suara ketikan (LOOPING) yang terdaftar di AudioManager")]
    public string defaultTypingSoundName;

    [Tooltip("Daftar tag untuk memutar suara spesifik dari AudioManager")]
    public List<CutsceneSoundTag> soundTags;

    void Start()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogError("AudioManager tidak ditemukan di scene! Cutscene tidak akan mengeluarkan suara.");
        }
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        foreach (string sentence in sentences)
        {
            yield return StartCoroutine(TypeSentence(sentence));
            yield return new WaitForSeconds(sentenceDelay);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // --- COROUTINE INI DIMODIFIKASI SESUAI PERMINTAAN ANDA ---
    IEnumerator TypeSentence(string sentence)
    {
        textDisplay.text = "";

        // --- MULAI mainkan suara ketikan looping ---
        if (!string.IsNullOrEmpty(defaultTypingSoundName))
        {
            AudioManager.instance.PlayLoopingSound(defaultTypingSoundName);
        }

        int i = 0;
        while (i < sentence.Length)
        {
            if (sentence[i] == '<' && sentence.Substring(i).StartsWith("<sfx:"))
            {
                int closingBracketIndex = sentence.IndexOf('>', i);
                if (closingBracketIndex > -1)
                {
                    int tagNameStartIndex = i + 5;
                    int tagNameLength = closingBracketIndex - tagNameStartIndex;
                    string tagName = sentence.Substring(tagNameStartIndex, tagNameLength);

                    PlaySoundForTag(tagName);

                    i = closingBracketIndex + 1;
                    continue;
                }
            }

            textDisplay.text += sentence[i];
            
            // Baris untuk memainkan suara per karakter DIHAPUS dari sini.
            
            i++;
            yield return new WaitForSeconds(typingSpeed);
        }

        // --- HENTIKAN suara ketikan looping setelah kalimat selesai ---
        if (!string.IsNullOrEmpty(defaultTypingSoundName))
        {
            AudioManager.instance.StopLoopingSound();
        }
    }

    void PlaySoundForTag(string tag)
    {
        foreach (var soundTag in soundTags)
        {
            if (string.Equals(soundTag.tag, tag, System.StringComparison.OrdinalIgnoreCase))
            {
                AudioManager.instance.PlaySFX(soundTag.soundName);
                return;
            }
        }
        Debug.LogWarning("Cutscene tag '" + tag + "' tidak ditemukan di daftar SoundTags pada CutsceneManager.");
    }
}