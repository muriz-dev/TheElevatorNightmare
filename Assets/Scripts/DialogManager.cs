using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public TextMeshProUGUI textDisplay;
    public float dialogTimer = 1f;

    private void Awake()
    {
        // --- Implementasi Singleton yang Benar ---
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Logika 'hasBeenTriggered' dihapus dari sini ---
    public void TriggerDialog(string dialogText)
    {
        // Hentikan coroutine yang mungkin sedang berjalan untuk menghindari tumpang tindih
        StopAllCoroutines();
        
        // Atur teks dan mulai coroutine baru
        textDisplay.text = dialogText;
        StartCoroutine(ShowDialog());
    }

    private IEnumerator ShowDialog()
    {
        textDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(dialogTimer);

        textDisplay.gameObject.SetActive(false);
    }
}