using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string dialogText;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Panggil manager untuk menampilkan dialog
            DialogManager.instance.TriggerDialog(dialogText);

            // --- Nonaktifkan pemicu ini agar tidak bisa digunakan lagi ---
            gameObject.SetActive(false);
        }
    }
}