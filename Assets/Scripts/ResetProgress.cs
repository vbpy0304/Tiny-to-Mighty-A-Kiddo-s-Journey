using System;
using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    public void ResetGame()
    {
        PlayerPrefs.DeleteAll(); // Clear all saved data
        PlayerPrefs.Save();
        Debug.Log("Progress Reset!");

        // Reload current scene so everything resets
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
