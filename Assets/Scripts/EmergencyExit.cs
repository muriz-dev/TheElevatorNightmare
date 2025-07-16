using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmergencyExit : MonoBehaviour
{
    public string endingSceneName;

    private void ExitGame()
    {
        SceneManager.LoadScene(endingSceneName);
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
