using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referensi Kamera")]
    [SerializeField] private Transform playerCamera;

    [Header("Pengaturan Interaksi")]
    [SerializeField] private float interactionDistance = 5f;

    // Referensi ke manager pusat
    [SerializeField] private ElevatorManager elevatorManager;

    private GameObject _lastLookedAtButton = null;

    private void Update()
    {
        if (elevatorManager == null) return;

        HandleLook();
        HandleClick();
    }

    private void HandleLook()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hitInfo;
        GameObject currentLookedAtButton = null;
        
        bool didHit = Physics.Raycast(ray, out hitInfo, interactionDistance);

        if (didHit)
        {
            Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.green);

            // Cek apakah objek yang dilihat punya tag "ElevatorInteract"
            if (hitInfo.collider.CompareTag("ElevatorInteract"))
            {
                currentLookedAtButton = hitInfo.collider.gameObject;
            }
            else if (hitInfo.collider.CompareTag("JumpscareCreature"))
            {
                // Jika kita melihat makhluk, coba panggil fungsinya
                JumpscareCreature creature = hitInfo.collider.GetComponent<JumpscareCreature>();
                if (creature != null)
                {
                    // Panggil fungsi pada makhluk, yang kemudian akan memberi tahu ElevatorManager
                    creature.OnPlayerLook();
                }
            }
            // --- LOGIKA BARU SELESAI ---
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
        }
        
        // Logika untuk tombol tidak berubah
        if (currentLookedAtButton != _lastLookedAtButton)
        {
            if (_lastLookedAtButton != null)
            {
                elevatorManager.OnLookAwayFromButton(_lastLookedAtButton);
            }

            if (currentLookedAtButton != null)
            {
                elevatorManager.OnLookAtButton(currentLookedAtButton);
            }

            _lastLookedAtButton = currentLookedAtButton;
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0) && _lastLookedAtButton != null)
        {
            elevatorManager.OnButtonPressed(_lastLookedAtButton);
        }
    }
}