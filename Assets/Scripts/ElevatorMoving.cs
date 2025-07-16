using UnityEngine;
using System.Collections; // Diperlukan untuk Coroutine

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

    public bool IsBroken { get; private set; } = false;

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

    // --- FUNGSI INI DIMODIFIKASI ---
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
            MoveToNextPoint = false; // Hentikan gerakan sebelum memulai sekuens baru
            StartCoroutine(ArrivalSequence()); // Mulai sekuens kedatangan
        }
    }

    // --- COROUTINE BARU UNTUK KEDATANGAN ---
    private IEnumerator ArrivalSequence()
    {
        if (_isCurrentlyMoving)
        {
            AudioManager.instance.StopLoopingSound();
            _isCurrentlyMoving = false;
        }

        // Ambil indeks titik yang baru saja dicapai SEBELUM menentukan target berikutnya.
        // Variabel _targetPointIndex saat ini masih menunjuk ke tujuan yang baru saja kita capai.
        int arrivedAtIndex = _targetPointIndex;

        // --- LOGIKA BARU: Cek apakah ini lantai yang rusak ---
        if (brokenFloorIndex != -1 && arrivedAtIndex == brokenFloorIndex)
        {
            Debug.Log("Elevator tiba di lantai rusak (" + arrivedAtIndex + "). Operasi dihentikan.");

            // Disarankan: Putar suara 'listrik mati' atau 'sabotase' di sini.
            // AudioManager.instance.PlaySFX("ElevatorBrokeSound");

            IsBroken = true; 

            // Tetap buka pintu agar pemain bisa keluar.
            if (doorController != null)
            {
                AudioManager.instance.PlaySFX(elevatorArriveSoundName);
                Debug.Log("PointToPointElevator: Putar suara 'ElevatorArrive'");

                yield return new WaitForSeconds(delayAfterArriveSound);

                doorController.OpenDoors();
            }

            // Hentikan coroutine di sini. Lift tidak akan bisa bergerak lagi karena
            // kita tidak memanggil UpdateNextTargetIndex().
            yield break;
        }
        // --- AKHIR LOGIKA BARU ---

        // Jika bukan lantai yang rusak, lanjutkan seperti biasa.
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