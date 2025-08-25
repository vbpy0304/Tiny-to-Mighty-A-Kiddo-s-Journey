using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltips : MonoBehaviour
{
    public GameObject tooltipPanel;   // Panel prefab (assign in Inspector)
    private AudioSource popAudio;     // Reference to pop sound

    private void Start()
    {
        // Disable tooltip at start
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        // Find GameObject tagged "pop" and get its AudioSource
        GameObject popObj = GameObject.FindGameObjectWithTag("pop");
        if (popObj != null)
        {
            popAudio = popObj.GetComponent<AudioSource>();
        }
    }

    public void ShowTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);

            // Play pop sound if available
            if (popAudio != null)
            {
                popAudio.Play();
            }
        }
    }
}
