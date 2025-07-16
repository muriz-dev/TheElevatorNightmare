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
    [SerializeField] private float delayAfterDoorCloses = 0.5f;
    [SerializeField] private float delayAfterButtonPress = 0.3f;
    [SerializeField] private float delayAfterArriveSound = 0.2f;
    
    [Header("Efek Suara")]
    [SerializeField] private string buttonPressSoundName = "ButtonClick";
    [SerializeField] private string elevatorArriveSoundName = "ElevatorArrive";

    // --- PERUBAHAN: PENGATURAN JUMPSCARE MENJADI LEBIH RAPI ---
    [Header("Pengaturan Jumpscare")]
    [Tooltip("Centang ini jika elevator ini memiliki fitur jumpscare.")]
    [SerializeField] private bool isJumpscareElevator = false;

    [Tooltip("Indeks lantai (dimulai dari 0) di mana jumpscare akan terjadi.")]
    [SerializeField] private int jumpscareFloorIndex = 0; 
    
    [Tooltip("Seret objek makhluk jumpscare DARI DALAM PREFAB INI ke sini.")]
    [SerializeField] private JumpscareCreature jumpscareCreature;
    
    [SerializeField] private string jumpscareSoundName = "MonsterScream";
    [SerializeField] private float delayAfterJumpscare = 2f;

    private bool isPlayerInside = false;
    private bool isSequenceRunning = false;
    private bool isWaitingForJumpscareLook = false;

    private void Start()
    {
        SetButtonVisibility(insideButton, false);
        SetButtonVisibility(outsideButton, false);
        
        // Hanya proses objek creature jika ini adalah elevator jumpscare dan referensinya ada
        if (isJumpscareElevator && jumpscareCreature != null)
        {
            jumpscareCreature.gameObject.SetActive(false);
        }
    }
    
    public void OnJumpscareTriggered()
    {
        // Tambahkan pengaman: hanya berjalan jika ini elevator jumpscare dan sedang menunggu
        if (!isJumpscareElevator || !isWaitingForJumpscareLook) return;

        isWaitingForJumpscareLook = false;
        StartCoroutine(JumpscareAftermathSequence());
    }
    
    private IEnumerator JumpscareAftermathSequence()
    {
        yield return new WaitForSeconds(delayAfterJumpscare);

        // if (jumpscareCreature != null)
        // {
        //     jumpscareCreature.gameObject.SetActive(false);
        // }
        
        StartCoroutine(CloseAndMoveSequence());
    }

    public void OnPlayerEntered()
    {
        isPlayerInside = true;
    }

    public void OnPlayerExited()
    {
        isPlayerInside = false;
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

        // --- PERUBAHAN: LOGIKA JUMPSCARE DIBUNGKUS DENGAN "SAKLAR" ---
        // Cek jika ini adalah elevator jumpscare SEBELUM memeriksa kondisi lainnya
        if (isJumpscareElevator && button == insideButton && isPlayerInside && elevatorMover.CurrentFloorIndex == jumpscareFloorIndex)
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
        
        // Jika bukan elevator jumpscare atau kondisinya tidak terpenuhi, jalankan alur normal
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