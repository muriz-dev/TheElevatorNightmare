using UnityEngine;

public class JumpscareCreature : MonoBehaviour
{
    // --- PERUBAHAN: Referensi tidak lagi di-set manual dari Inspector ---
    private ElevatorManager _manager;
    private bool hasBeenTriggered = false;

    void Awake()
    {
        // Cari ElevatorManager di parent saat objek ini pertama kali aktif
        _manager = GetComponentInParent<ElevatorManager>();
        if (_manager == null)
        {
            Debug.LogError("JumpscareCreature tidak dapat menemukan ElevatorManager di parent-nya!", this.gameObject);
        }
    }

    public void OnPlayerLook()
    {
        if (!hasBeenTriggered && _manager != null)
        {
            hasBeenTriggered = true;
            _manager.OnJumpscareTriggered();
        }
    }
}