using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referensi Kamera")]
    [SerializeField] private Transform playerCamera;

    [Header("Pengaturan Interaksi")]
    [SerializeField] private float interactionDistance = 5f;

    // --- DIHAPUS ---
    // [SerializeField] private ElevatorManager elevatorManager; 

    private GameObject _lastLookedAtButton = null;
    // --- VARIABEL BARU ---
    private ElevatorManager _lastManager = null;

    private void Update()
    {
        // Tidak perlu lagi 'if (elevatorManager == null) return;'
        HandleLook();
        HandleClick();
    }

    private void HandleLook()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hitInfo;
        GameObject currentLookedAtButton = null;
        ElevatorManager currentManager = null; // Variabel sementara

        bool didHit = Physics.Raycast(ray, out hitInfo, interactionDistance);

        if (didHit)
        {
            Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.green);

            if (hitInfo.collider.CompareTag("ElevatorInteract"))
            {
                currentLookedAtButton = hitInfo.collider.gameObject;
                // --- LOGIKA BARU: Ambil manager dari tombol ---
                currentManager = currentLookedAtButton.GetComponentInParent<ElevatorManager>();
            }
            else if (hitInfo.collider.CompareTag("JumpscareCreature"))
            {
                JumpscareCreature creature = hitInfo.collider.GetComponent<JumpscareCreature>();
                if (creature != null)
                {
                    creature.OnPlayerLook();
                }
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
        }

        if (currentLookedAtButton != _lastLookedAtButton)
        {
            // Beri tahu manager LAMA kita berhenti melihat tombol lama
            if (_lastLookedAtButton != null && _lastManager != null)
            {
                _lastManager.OnLookAwayFromButton(_lastLookedAtButton);
            }

            // Beri tahu manager BARU kita sekarang melihat tombol baru
            if (currentLookedAtButton != null && currentManager != null)
            {
                currentManager.OnLookAtButton(currentLookedAtButton);
            }

            _lastLookedAtButton = currentLookedAtButton;
            _lastManager = currentManager; // Simpan manager yang baru
        }
    }

    private void HandleClick()
    {
        // Jika kita klik saat sedang melihat sebuah tombol dan managernya ada
        if (Input.GetMouseButtonDown(0) && _lastLookedAtButton != null && _lastManager != null)
        {
            _lastManager.OnButtonPressed(_lastLookedAtButton);
        }
    }
}