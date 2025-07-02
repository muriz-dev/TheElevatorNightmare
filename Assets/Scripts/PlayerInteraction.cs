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
        
        // --- LOGIKA DEBUG VISUAL DIMULAI DI SINI ---
        
        bool didHit = Physics.Raycast(ray, out hitInfo, interactionDistance);

        if (didHit)
        {
            // Jika mengenai sesuatu, gambar garis hijau
            Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.green);

            // Cek apakah objek yang dilihat punya tag "ElevatorInteract"
            if (hitInfo.collider.CompareTag("ElevatorInteract"))
            {
                currentLookedAtButton = hitInfo.collider.gameObject;
            }
        }
        else
        {
            // Jika tidak mengenai apa-apa, gambar garis merah
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
        }
        
        // --- LOGIKA DEBUG VISUAL SELESAI ---


        // Jika ada perubahan dari apa yang kita lihat sebelumnya
        if (currentLookedAtButton != _lastLookedAtButton)
        {
            // Beri tahu manager kita berhenti melihat tombol lama
            if (_lastLookedAtButton != null)
            {
                elevatorManager.OnLookAwayFromButton(_lastLookedAtButton);
            }

            // Beri tahu manager kita sekarang melihat tombol baru
            if (currentLookedAtButton != null)
            {
                elevatorManager.OnLookAtButton(currentLookedAtButton);
            }

            _lastLookedAtButton = currentLookedAtButton;
        }
    }

    private void HandleClick()
    {
        // Jika kita klik saat sedang melihat sebuah tombol
        if (Input.GetMouseButtonDown(0) && _lastLookedAtButton != null)
        {
            elevatorManager.OnButtonPressed(_lastLookedAtButton);
        }
    }
}
