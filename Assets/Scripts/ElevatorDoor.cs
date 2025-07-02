using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    [Header("Referensi Pintu")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Pengaturan Gerakan")]
    [SerializeField] private float openOffset = 1.2f;
    [SerializeField] private float doorMoveDuration = 1.5f;

    public float DoorMoveDuration => doorMoveDuration;

    // --- PROPERTI BARU UNTUK MELACAK STATUS PINTU ---
    public bool IsOpen { get; private set; } = false;

    // TAMBAHKAN PROPERTI INI
    public bool IsMoving => _isDoorMoving;

    private Vector3 _leftDoorClosedPosition;
    private Vector3 _rightDoorClosedPosition;
    private bool _isDoorMoving = false;

    private void Start()
    {
        if (leftDoor != null) { _leftDoorClosedPosition = leftDoor.localPosition; }
        if (rightDoor != null) { _rightDoorClosedPosition = rightDoor.localPosition; }
        // Kondisi awal pintu adalah tertutup
        IsOpen = false;
    }

    public void OpenDoors()
    {
        Vector3 leftDoorOpenPosition = _leftDoorClosedPosition + Vector3.left * openOffset;
        Vector3 rightDoorOpenPosition = _rightDoorClosedPosition + Vector3.right * openOffset;
        // Beri tahu coroutine bahwa kita sedang membuka pintu (true)
        StartMoveDoors(leftDoorOpenPosition, rightDoorOpenPosition, true);
    }

    public void CloseDoors()
    {
        // Beri tahu coroutine bahwa kita sedang menutup pintu (false)
        StartMoveDoors(_leftDoorClosedPosition, _rightDoorClosedPosition, false);
    }

    private void StartMoveDoors(Vector3 leftTarget, Vector3 rightTarget, bool opens)
    {
        if (!_isDoorMoving)
        {
            StartCoroutine(MoveDoorsCoroutine(leftTarget, rightTarget, opens));
        }
    }

    private IEnumerator MoveDoorsCoroutine(Vector3 leftTargetPos, Vector3 rightTargetPos, bool opens)
    {
        _isDoorMoving = true;
        float elapsedTime = 0f;
        Vector3 startPosLeft = leftDoor.localPosition;
        Vector3 startPosRight = rightDoor.localPosition;

        while (elapsedTime < doorMoveDuration)
        {
            float percentageComplete = elapsedTime / doorMoveDuration;
            leftDoor.localPosition = Vector3.Lerp(startPosLeft, leftTargetPos, percentageComplete);
            rightDoor.localPosition = Vector3.Lerp(startPosRight, rightTargetPos, percentageComplete);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leftDoor.localPosition = leftTargetPos;
        rightDoor.localPosition = rightTargetPos;
        
        // --- PERBARUI STATUS SETELAH GERAKAN SELESAI ---
        IsOpen = opens;
        _isDoorMoving = false;
    }
}
