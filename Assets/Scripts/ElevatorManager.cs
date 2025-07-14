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

    // --- TAMBAHAN BARU: Jeda setelah tombol ditekan ---
    [Tooltip("Jeda waktu (dalam detik) setelah tombol ditekan sebelum pintu bereaksi. Beri nilai kecil seperti 0.3")]
    [SerializeField] private float delayAfterButtonPress = 0.3f;
    // --- AKHIR TAMBAHAN ---


    [Header("Efek Suara")]
    [Tooltip("Nama SFX yang diputar saat tombol ditekan.")]
    [SerializeField] private string buttonPressSoundName = "ButtonClick";

    private bool isPlayerInside = false;
    private bool isSequenceRunning = false; // Flag untuk mencegah klik ganda

    private void Start() //
    {
        SetButtonVisibility(insideButton, false); //
        SetButtonVisibility(outsideButton, false); //
    }

    public void OnPlayerEntered() //
    {
        isPlayerInside = true; //
    }

    public void OnLookAtButton(GameObject button) //
    {
        if (button == outsideButton && !isPlayerInside && !doorController.IsOpen && !doorController.IsMoving) //
        {
            SetButtonVisibility(button, true); //
        }
        else if (button == insideButton && isPlayerInside && !elevatorMover.MoveToNextPoint && !doorController.IsMoving) //
        {
            SetButtonVisibility(button, true); //
        }
    }

    public void OnLookAwayFromButton(GameObject button) //
    {
        SetButtonVisibility(button, false); //
    }

    // --- FUNGSI INI DIMODIFIKASI ---
    public void OnButtonPressed(GameObject button) //
    {
        // Mencegah pemain menekan tombol lagi jika sekuens sudah berjalan
        if (isSequenceRunning)
        {
            return;
        }
        StartCoroutine(ButtonPressedSequence(button));
    }

    // --- COROUTINE BARU UNTUK MENGELOLA URUTAN AKSI ---
    private IEnumerator ButtonPressedSequence(GameObject button)
    {
        isSequenceRunning = true;

        // 1. Langsung sembunyikan tombol dan putar suara
        SetButtonVisibility(button, false);
        if (!string.IsNullOrEmpty(buttonPressSoundName)) //
        {
            AudioManager.instance.PlaySFX(buttonPressSoundName); //
        }

        // 2. Tunggu sejenak agar suara tombol tidak tertimpa
        yield return new WaitForSeconds(delayAfterButtonPress);

        // 3. Jalankan aksi pintu setelah jeda
        if (button == outsideButton && !isPlayerInside) //
        {
            doorController.OpenDoors(); //
        }
        else if (button == insideButton && isPlayerInside) //
        {
            StartCoroutine(CloseAndMoveSequence()); //
        }

        // 4. Izinkan tombol ditekan lagi
        isSequenceRunning = false;
    }

    private IEnumerator CloseAndMoveSequence() //
    {
        // Tombol sudah disembunyikan sebelumnya, jadi baris ini bisa dihapus jika mau
        SetButtonVisibility(insideButton, false); //
        doorController.CloseDoors(); //
        yield return new WaitForSeconds(doorController.DoorMoveDuration); //
        
        yield return new WaitForSeconds(delayAfterDoorCloses); //

        elevatorMover.MoveToNextPoint = true; //
    }

    private void SetButtonVisibility(GameObject buttonParent, bool isVisible) //
    {
        if (buttonParent != null) //
        {
            Transform visuals = buttonParent.transform.Find("Button"); //
            if (visuals != null) //
            {
                visuals.gameObject.SetActive(isVisible); //
            }
            else
            {
                Debug.LogWarning("Objek 'Visuals' tidak ditemukan sebagai anak dari " + buttonParent.name); //
            }
        }
    }
}