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

    [Header("Komponen Terhubung")]
    [Tooltip("Seret komponen ElevatorDoor ke sini.")]
    [SerializeField] private ElevatorDoor doorController;

    // --- TAMBAHAN BARU: Nama SFX untuk gerakan ---
    [Header("Efek Suara")]
    [Tooltip("Nama SFX looping yang diputar saat lift bergerak.")]
    [SerializeField] private string movingSoundName = "ElevatorMovingLoop";

    [Header("Kontrol Gerakan")]
    public bool MoveToNextPoint = false;

    private int _targetPointIndex;
    private bool _isMovingForward = true;
    private bool _isCurrentlyMoving = false; // Variabel untuk melacak status suara

    public Vector3 CurrentTargetPosition => pathPoints[_targetPointIndex].position;

    private void Start()
    {
        // ... (Fungsi Start tidak berubah)
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
        // Jika lift tidak diperintahkan bergerak
        if (!MoveToNextPoint)
        {
            // Pastikan suara berhenti jika lift tiba-tiba dihentikan
            if (_isCurrentlyMoving)
            {
                AudioManager.instance.StopLoopingSound();
                _isCurrentlyMoving = false;
            }
            return;
        }

        // --- LOGIKA BARU: Memulai suara saat pertama kali bergerak ---
        if (!_isCurrentlyMoving)
        {
            if (!string.IsNullOrEmpty(movingSoundName))
            {
                AudioManager.instance.PlayLoopingSound(movingSoundName);
            }
            _isCurrentlyMoving = true;
        }
        // --- AKHIR LOGIKA BARU ---

        transform.position = Vector3.MoveTowards(transform.position, CurrentTargetPosition, moveSpeed * Time.deltaTime);

        // Saat lift tiba di tujuan
        if (Vector3.Distance(transform.position, CurrentTargetPosition) < 0.01f)
        {
            // --- LOGIKA BARU: Hentikan suara saat tiba ---
            if (_isCurrentlyMoving)
            {
                AudioManager.instance.StopLoopingSound();
                _isCurrentlyMoving = false;
            }
            // --- AKHIR LOGIKA BARU ---

            MoveToNextPoint = false;
            UpdateNextTargetIndex();
            
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
        // ... (Fungsi InitializeElevator tidak berubah)
        startPointIndex = Mathf.Clamp(startPointIndex, 0, pathPoints.Length - 1);
        transform.position = pathPoints[startPointIndex].position;
        _targetPointIndex = startPointIndex;
        if (_targetPointIndex >= pathPoints.Length - 1) _isMovingForward = false;
        else if (_targetPointIndex <= 0) _isMovingForward = true;
        UpdateNextTargetIndex();
    }

    private void UpdateNextTargetIndex()
    {
        // ... (Fungsi UpdateNextTargetIndex tidak berubah)
        if (_targetPointIndex >= pathPoints.Length - 1) _isMovingForward = false;
        else if (_targetPointIndex <= 0) _isMovingForward = true;

        if (_isMovingForward) _targetPointIndex++;
        else _targetPointIndex--;
    }
}