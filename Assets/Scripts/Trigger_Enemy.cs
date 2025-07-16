using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Trigger_Enemy : MonoBehaviour
{
    private Animator mAnimator;
    public GameObject gameOverPanel;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered with: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone.");
            mAnimator.SetTrigger("TrStop");
            gameOverPanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
}


}
