using UnityEngine;

/// <summary>
/// Menggerakkan platform dari satu titik ke titik berikutnya berdasarkan perintah eksternal.
/// Setelah sampai di satu titik, platform akan berhenti dan menunggu perintah selanjutnya.
/// </summary>
public class PointToPointElevator : MonoBehaviour
{
    [Header("Pengaturan Gerakan")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Titik Jalur")]
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private int startPointIndex = 0;

    // --- REFERENSI BARU ---
    [Header("Komponen Terhubung")]
    [Tooltip("Seret komponen ElevatorDoor ke sini.")]
    [SerializeField] private ElevatorDoor doorController;

    [Header("Kontrol Gerakan")]
    public bool MoveToNextPoint = false;

    private int _targetPointIndex;
    private bool _isMovingForward = true;

    public Vector3 CurrentTargetPosition => pathPoints[_targetPointIndex].position;

    private void Start()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("Elevator memerlukan setidaknya 2 titik jalur.", this.gameObject);
            enabled = false;
            return;
        }
        InitializeElevator();
    }

    private void FixedUpdate()
    {
        if (!MoveToNextPoint) return;

        transform.position = Vector3.MoveTowards(transform.position, CurrentTargetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, CurrentTargetPosition) < 0.01f)
        {
            MoveToNextPoint = false;
            UpdateNextTargetIndex();

            // --- LOGIKA BARU: Buka pintu saat tiba ---
            if (doorController != null)
            {
                doorController.OpenDoors();
            }
            else
            {
                Debug.LogWarning("Door Controller tidak terhubung di PointToPointElevator.", this.gameObject);
            }
        }
    }

    private void InitializeElevator()
    {
        startPointIndex = Mathf.Clamp(startPointIndex, 0, pathPoints.Length - 1);
        transform.position = pathPoints[startPointIndex].position;
        _targetPointIndex = startPointIndex;
        if (_targetPointIndex >= pathPoints.Length - 1) _isMovingForward = false;
        else if (_targetPointIndex <= 0) _isMovingForward = true;
        UpdateNextTargetIndex();
    }

    private void UpdateNextTargetIndex()
    {
        if (_targetPointIndex >= pathPoints.Length - 1) _isMovingForward = false;
        else if (_targetPointIndex <= 0) _isMovingForward = true;

        if (_isMovingForward) _targetPointIndex++;
        else _targetPointIndex--;
    }
}