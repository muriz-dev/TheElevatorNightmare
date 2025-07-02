using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [Header("Komponen Inti")]
    [SerializeField] private PointToPointElevator elevatorMover;
    [SerializeField] private ElevatorDoor doorController;

    [Header("Tombol Interaksi")]
    [Tooltip("GameObject tombol yang ada di luar elevator (objek induknya).")]
    [SerializeField] private GameObject outsideButton;
    [Tooltip("GameObject tombol yang ada di dalam elevator (objek induknya).")]
    [SerializeField] private GameObject insideButton;

    [Header("Pengaturan Waktu")]
    [Tooltip("Jeda waktu (dalam detik) setelah pintu tertutup sebelum elevator bergerak.")]
    [SerializeField] private float delayAfterDoorCloses = 0.5f;

    private bool isPlayerInside = false;

    private void Start()
    {
        SetButtonVisibility(insideButton, false);
        SetButtonVisibility(outsideButton, false);
    }

    public void OnPlayerEntered()
    {
        isPlayerInside = true;
    }

    // public void OnPlayerExited()
    // {
    //     isPlayerInside = false;
    //     doorController.CloseDoors();
    //     SetButtonVisibility(insideButton, false);
    // }

    public void OnLookAtButton(GameObject button)
    {
        // Tampilkan tombol luar HANYA JIKA pemain di luar DAN pintu sedang tertutup.
        if (button == outsideButton && !isPlayerInside && !doorController.IsOpen && !doorController.IsMoving)
        {
            SetButtonVisibility(button, true);
        }
        // --- MODIFIKASI LOGIKA DI BAWAH INI ---
        else if (button == insideButton && isPlayerInside && !elevatorMover.MoveToNextPoint && !doorController.IsMoving)
        {
            SetButtonVisibility(button, true);
        }
    }

    public void OnLookAwayFromButton(GameObject button)
    {
        SetButtonVisibility(button, false);
    }

    public void OnButtonPressed(GameObject button)
    {
        if (button == outsideButton && !isPlayerInside)
        {
            doorController.OpenDoors();
            SetButtonVisibility(button, false);
        }
        else if (button == insideButton && isPlayerInside)
        {
            StartCoroutine(CloseAndMoveSequence());
        }
    }

    private IEnumerator CloseAndMoveSequence()
    {
        SetButtonVisibility(insideButton, false);
        doorController.CloseDoors();
        yield return new WaitForSeconds(doorController.DoorMoveDuration);

        // Tunggu sejenak sesuai durasi yang ditentukan
        yield return new WaitForSeconds(delayAfterDoorCloses);

        elevatorMover.MoveToNextPoint = true;
    }

    private void SetButtonVisibility(GameObject buttonParent, bool isVisible)
    {
        if (buttonParent != null)
        {
            Transform visuals = buttonParent.transform.Find("Button");
            if (visuals != null)
            {
                visuals.gameObject.SetActive(isVisible);
            }
            else
            {
                Debug.LogWarning("Objek 'Visuals' tidak ditemukan sebagai anak dari " + buttonParent.name);
            }
        }
    }
}
