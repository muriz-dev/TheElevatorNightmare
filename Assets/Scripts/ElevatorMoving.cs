using UnityEngine;
using System.Collections;

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

    [Header("Pengaturan Kerusakan")]
    [Tooltip("Indeks titik jalur (lantai) di mana elevator akan berhenti berfungsi. Isi -1 jika tidak ada lantai yang rusak.")]
    [SerializeField] private int brokenFloorIndex = -1;

    [Header("Komponen Terhubung")]
    [Tooltip("Seret komponen ElevatorDoor ke sini.")]
    [SerializeField] private ElevatorDoor doorController;

    [Header("Pengaturan Waktu & Suara")]
    [Tooltip("Jeda setelah suara 'arrive' diputar sebelum pintu terbuka otomatis.")]
    [SerializeField] private float delayAfterArriveSound = 0.2f;
    [Tooltip("Nama SFX yang diputar saat pintu elevator terbuka.")]
    [SerializeField] private string elevatorArriveSoundName = "ElevatorArrive";
    [Tooltip("Nama SFX looping yang diputar saat lift bergerak.")]
    [SerializeField] private string movingSoundName = "ElevatorMovingLoop";

    [Header("Kontrol Gerakan")]
    public bool MoveToNextPoint = false;

    private int _targetPointIndex;
    private bool _isMovingForward = true;
    private bool _isCurrentlyMoving = false;

    // --- VARIABEL BARU ---
    // Variabel untuk menyimpan indeks lantai tempat elevator tiba
    private int _arrivedAtIndex = 0;

    public bool IsBroken { get; private set; } = false;
    
    // --- PROPERTI BARU ---
    // Properti publik agar skrip lain bisa membaca lantai saat ini
    public int CurrentFloorIndex => _arrivedAtIndex;
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
        if (!MoveToNextPoint)
        {
            if (_isCurrentlyMoving)
            {
                AudioManager.instance.StopLoopingSound();
                _isCurrentlyMoving = false;
            }
            return;
        }

        if (!_isCurrentlyMoving)
        {
            if (!string.IsNullOrEmpty(movingSoundName))
            {
                AudioManager.instance.PlayLoopingSound(movingSoundName);
                Debug.Log("PointToPointElevator: Putar suara 'ElevatorRide'");
            }
            _isCurrentlyMoving = true;
        }

        transform.position = Vector3.MoveTowards(transform.position, CurrentTargetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, CurrentTargetPosition) < 0.01f)
        {
            MoveToNextPoint = false;
            StartCoroutine(ArrivalSequence());
        }
    }

    private IEnumerator ArrivalSequence()
    {
        if (_isCurrentlyMoving)
        {
            AudioManager.instance.StopLoopingSound();
            _isCurrentlyMoving = false;
        }

        // --- MODIFIKASI: Simpan indeks lantai tempat kita tiba ---
        _arrivedAtIndex = _targetPointIndex;

        if (brokenFloorIndex != -1 && _arrivedAtIndex == brokenFloorIndex)
        {
            Debug.Log("Elevator tiba di lantai rusak (" + _arrivedAtIndex + "). Operasi dihentikan.");
            IsBroken = true; 
            if (doorController != null)
            {
                AudioManager.instance.PlaySFX(elevatorArriveSoundName);
                Debug.Log("PointToPointElevator: Putar suara 'ElevatorArrive'");
                yield return new WaitForSeconds(delayAfterArriveSound);
                doorController.OpenDoors();
            }
            yield break;
        }

        UpdateNextTargetIndex();

        AudioManager.instance.PlaySFX(elevatorArriveSoundName);
        Debug.Log("PointToPointElevator: Putar suara 'ElevatorArrive'");
        yield return new WaitForSeconds(delayAfterArriveSound);

        if (doorController != null)
        {
            doorController.OpenDoors();
        }
        else
        {
            Debug.LogWarning("Door Controller tidak terhubung di PointToPointElevator.", this.gameObject);
        }
    }

    private void InitializeElevator()
    {
        startPointIndex = Mathf.Clamp(startPointIndex, 0, pathPoints.Length - 1);
        transform.position = pathPoints[startPointIndex].position;
        _targetPointIndex = startPointIndex;
        // --- MODIFIKASI: Simpan juga indeks awal ---
        _arrivedAtIndex = startPointIndex;
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