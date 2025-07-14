using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [Header("Komponen Inti")]
    [SerializeField] private PointToPointElevator elevatorMover;
    [SerializeField] private ElevatorDoor doorController;

    [Header("Tombol Interaksi")]
    [SerializeField] private GameObject outsideButton;
    [SerializeField] private GameObject insideButton;

    [Header("Pengaturan Waktu")]
    [Tooltip("Jeda waktu (dalam detik) setelah pintu tertutup sebelum elevator bergerak.")]
    [SerializeField] private float delayAfterDoorCloses = 0.5f;

    [Tooltip("Jeda waktu (dalam detik) setelah tombol ditekan sebelum pintu bereaksi. Beri nilai kecil seperti 0.3")]
    [SerializeField] private float delayAfterButtonPress = 0.3f;

    // --- TAMBAHAN BARU ---
    [Tooltip("Jeda waktu setelah suara 'arrive' diputar sebelum pintu mulai terbuka.")]
    [SerializeField] private float delayAfterArriveSound = 0.2f;
    // --- AKHIR TAMBAHAN ---


    [Header("Efek Suara")]
    [Tooltip("Nama SFX yang diputar saat tombol ditekan.")]
    [SerializeField] private string buttonPressSoundName = "ButtonClick";
    [Tooltip("Nama SFX yang diputar saat pintu elevator terbuka.")]
    [SerializeField] private string elevatorArriveSoundName = "ElevatorArrive";

    private bool isPlayerInside = false;
    private bool isSequenceRunning = false;

    private void Start()
    {
        SetButtonVisibility(insideButton, false);
        SetButtonVisibility(outsideButton, false);
    }

    public void OnPlayerEntered()
    {
        isPlayerInside = true;
    }

    public void OnLookAtButton(GameObject button)
    {
        if (button == outsideButton && !isPlayerInside && !doorController.IsOpen && !doorController.IsMoving)
        {
            SetButtonVisibility(button, true);
        }
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
        if (isSequenceRunning)
        {
            return;
        }
        StartCoroutine(ButtonPressedSequence(button));
    }
    
    // --- COROUTINE INI YANG DIMODIFIKASI ---
    private IEnumerator ButtonPressedSequence(GameObject button)
    {
        isSequenceRunning = true;

        SetButtonVisibility(button, false);
        if (!string.IsNullOrEmpty(buttonPressSoundName))
        {
            AudioManager.instance.PlaySFX(buttonPressSoundName);

            Debug.Log("ElevatorManager: Putar suara 'ButtonClick'");
        }

        yield return new WaitForSeconds(delayAfterButtonPress);

        if (button == outsideButton && !isPlayerInside)
        {
            // 1. Putar suara kedatangan lift
            AudioManager.instance.PlaySFX(elevatorArriveSoundName);

            Debug.Log("ElevatorManager: Putar suara 'ElevatorArrive'");

            // 2. Tunggu sejenak setelah suara diputar
            yield return new WaitForSeconds(delayAfterArriveSound);

            // 3. Baru buka pintu
            doorController.OpenDoors();
        }
        else if (button == insideButton && isPlayerInside)
        {
            StartCoroutine(CloseAndMoveSequence());
        }

        isSequenceRunning = false;
    }

    private IEnumerator CloseAndMoveSequence()
    {
        SetButtonVisibility(insideButton, false);
        doorController.CloseDoors();
        yield return new WaitForSeconds(doorController.DoorMoveDuration);

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