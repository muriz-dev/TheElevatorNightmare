using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public TextMeshProUGUI textDisplay;
    public string earlyQuestText;

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
            return;
        }

        textDisplay.text = earlyQuestText;
    }

    // Fungsi ini sekarang bisa dipanggil berkali-kali dari trigger yang berbeda
    public void ChangeQuestText(string newText)
    {
        textDisplay.text = newText;
    }
}