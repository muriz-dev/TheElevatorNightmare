using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPlatform : MonoBehaviour
{
    // Referensi ke manager pusat
    [SerializeField] private ElevatorManager elevatorManager;

    void OnTriggerEnter(Collider other)
    {
        // Hanya bereaksi pada player (asumsikan player punya tag "Player")
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
            if (elevatorManager != null)
            {
                elevatorManager.OnPlayerEntered();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
            // if (elevatorManager != null)
            // {
            //     elevatorManager.OnPlayerExited();
            // }
        }
    }
}
