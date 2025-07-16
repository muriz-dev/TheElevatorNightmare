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
    [Tooltip("Jeda waktu setelah suara 'arrive' diputar sebelum pintu mulai terbuka.")]
    [SerializeField] private float delayAfterArriveSound = 0.2f;
    
    [Header("Efek Suara")]
    [Tooltip("Nama SFX yang diputar saat tombol ditekan.")]
    [SerializeField] private string buttonPressSoundName = "ButtonClick";
    [Tooltip("Nama SFX yang diputar saat pintu elevator terbuka.")]
    [SerializeField] private string elevatorArriveSoundName = "ElevatorArrive";

    [Header("Pengaturan Jumpscare")]
    [Tooltip("Indeks lantai (dimulai dari 0) di mana jumpscare akan terjadi.")]
    [SerializeField] private int jumpscareFloorIndex = 3; 
    [Tooltip("Seret objek makhluk jumpscare ke sini.")]
    [SerializeField] private JumpscareCreature jumpscareCreature;
    [Tooltip("Nama SFX yang akan diputar saat jumpscare terjadi.")]
    [SerializeField] private string jumpscareSoundName = "MonsterScream";
    
    // --- VARIABEL BARU ---
    [Tooltip("Jeda waktu (dalam detik) setelah pemain melihat makhluk sebelum pintu tertutup.")]
    [SerializeField] private float delayAfterJumpscare = 2f;

    private bool isPlayerInside = false;
    private bool isSequenceRunning = false;
    private bool isWaitingForJumpscareLook = false;


    private void Start()
    {
        SetButtonVisibility(insideButton, false);
        SetButtonVisibility(outsideButton, false);
        if (jumpscareCreature != null)
        {
            jumpscareCreature.gameObject.SetActive(false);
        }
    }

    // --- FUNGSI INI DIMODIFIKASI ---
    /// <summary>
    /// Fungsi ini sekarang memanggil coroutine baru yang memiliki jeda.
    /// </summary>
    public void OnJumpscareTriggered()
    {
        if (!isWaitingForJumpscareLook) return;

        isWaitingForJumpscareLook = false;
        
        // Memulai sekuens setelah jumpscare, yang berisi delay
        StartCoroutine(JumpscareAftermathSequence());
    }
    
    // --- COROUTINE BARU ---
    /// <summary>
    /// Coroutine ini menangani apa yang terjadi setelah pemain melihat makhluk.
    /// </summary>
    private IEnumerator JumpscareAftermathSequence()
    {
        // Beri jeda waktu untuk menambah ketegangan. Makhluk masih terlihat.
        yield return new WaitForSeconds(delayAfterJumpscare);

        // Sembunyikan kembali makhluknya setelah jeda selesai
        // if (jumpscareCreature != null)
        // {
        //     jumpscareCreature.gameObject.SetActive(false);
        // }

        // Mulai urutan menutup pintu dan bergerak
        StartCoroutine(CloseAndMoveSequence());
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
        if (isSequenceRunning) return;
        StartCoroutine(ButtonPressedSequence(button));
    }

    private IEnumerator ButtonPressedSequence(GameObject button)
    {
        isSequenceRunning = true;

        if (elevatorMover != null && elevatorMover.IsBroken && button == insideButton)
        {
            Debug.Log("Tombol ditekan, tetapi lift rusak.");
            if (!string.IsNullOrEmpty(buttonPressSoundName))
            {
                AudioManager.instance.PlaySFX(buttonPressSoundName);
                yield return new WaitForSeconds(delayAfterButtonPress);
                AudioManager.instance.PlaySFX("ElevatorError");
            }
            isSequenceRunning = false;
            yield break;
        }
        
        SetButtonVisibility(button, false);
        if (!string.IsNullOrEmpty(buttonPressSoundName))
        {
            AudioManager.instance.PlaySFX(buttonPressSoundName);
        }
        yield return new WaitForSeconds(delayAfterButtonPress);

        if (button == insideButton && isPlayerInside && elevatorMover.CurrentFloorIndex == jumpscareFloorIndex)
        {
            if (jumpscareCreature != null)
            {
                jumpscareCreature.gameObject.SetActive(true);
                AudioManager.instance.PlaySFX(jumpscareSoundName);
                isWaitingForJumpscareLook = true;
                isSequenceRunning = false;
                yield break; 
            }
        }
        
        if (button == outsideButton && !isPlayerInside)
        {
            AudioManager.instance.PlaySFX(elevatorArriveSoundName);
            yield return new WaitForSeconds(delayAfterArriveSound);
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