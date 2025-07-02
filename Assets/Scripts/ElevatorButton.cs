using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [Header("Referensi Elevator")]
    [SerializeField] private PointToPointElevator elevatorScript;

    // --- REFERENSI BARU ---
    [Header("Komponen Terhubung")]
    [Tooltip("Seret komponen ElevatorDoor ke sini.")]
    [SerializeField] private ElevatorDoor doorController;
    [Tooltip("Seret GameObject Tombol itu sendiri ke sini.")]
    [SerializeField] private GameObject buttonObject;

    private bool _isBeingLookedAt = false;
    private bool _isCallingElevator = false; // Flag untuk mencegah klik ganda

    private void Update()
    {
        if (elevatorScript == null || buttonObject == null) return;

        bool canBeSeen = !_isCallingElevator;
        bool shouldBeActive = _isBeingLookedAt && !elevatorScript.MoveToNextPoint && canBeSeen;

        if (buttonObject.activeSelf != shouldBeActive)
        {
            buttonObject.SetActive(shouldBeActive);
        }
    }

    public void SetLookStatus(bool isLookedAt)
    {
        _isBeingLookedAt = isLookedAt;
    }

    public void CallElevatorToNextPoint()
    {
        if (doorController == null || elevatorScript == null)
        {
            Debug.LogError("Referensi Pintu atau Elevator belum diatur!", this.gameObject);
            return;
        }

        if (!elevatorScript.MoveToNextPoint && !_isCallingElevator)
        {
            StartCoroutine(CloseDoorsAndMoveElevator());
        }
    }

    private IEnumerator CloseDoorsAndMoveElevator()
    {
        _isCallingElevator = true;

        doorController.CloseDoors();

        yield return new WaitForSeconds(doorController.DoorMoveDuration);

        elevatorScript.MoveToNextPoint = true;

        _isCallingElevator = false;
    }
}
