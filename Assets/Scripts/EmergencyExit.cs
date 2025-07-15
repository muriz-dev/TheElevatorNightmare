using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmergencyExit : MonoBehaviour
{
    private void ExitGame()
    {
        SceneManager.LoadScene("Ending");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ExitGame();
        }

        Debug.Log("Collision with: " + other.gameObject.name);   
    }
}
