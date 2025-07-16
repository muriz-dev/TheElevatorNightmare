using UnityEngine;

/// <summary>
/// Skrip ini ditempelkan pada objek makhluk jumpscare.
/// Tugasnya adalah memberi tahu ElevatorManager ketika pemain melihatnya.
/// </summary>
public class JumpscareCreature : MonoBehaviour
{
    // Referensi ke manager elevator utama
    [SerializeField] private ElevatorManager elevatorManager;

    // Variabel untuk memastikan trigger hanya berjalan sekali
    private bool hasBeenTriggered = false;

    /// <summary>
    /// Fungsi ini akan dipanggil oleh PlayerInteraction ketika
    /// raycast dari pemain mengenai objek ini.
    /// </summary>
    public void OnPlayerLook()
    {
        Debug.Log("JumpscareCreature: Pemain melihat makhluk jumpscare.");
        // Hanya jalankan jika belum pernah ditrigger dan referensi manager ada
        if (!hasBeenTriggered && elevatorManager != null)
        {
            hasBeenTriggered = true;

            // Beri tahu manager bahwa jumpscare telah berhasil (pemain sudah melihat)
            elevatorManager.OnJumpscareTriggered();
        }
    }
}