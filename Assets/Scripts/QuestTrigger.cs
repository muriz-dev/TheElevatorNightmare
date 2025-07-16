using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public string questText;

    // --- Menggunakan OnTriggerEnter agar konsisten ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Panggil manager untuk mengubah teks
            QuestManager.instance.ChangeQuestText(questText);
            
            // --- Nonaktifkan pemicu ini agar tidak bisa digunakan lagi ---
            gameObject.SetActive(false);
        }
    }
}