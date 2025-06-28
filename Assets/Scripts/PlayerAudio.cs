using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    void PlayWalkFootstep()
    {
        AudioManager.instance.PlaySFX("WalkFootstep");
    }

    void PlayRunFootstep()
    {
        AudioManager.instance.PlaySFX("RunFootstep");
    }
}
