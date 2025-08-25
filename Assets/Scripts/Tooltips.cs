using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltips : MonoBehaviour
{
    public GameObject tooltipPanel;   // Panel prefab
    private AudioSource popAudio;     // Reference to pop sound object

    private void Start()
    {
        // Disable tooltip at start
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        // Find GameObject tagged "pop" and get its audio source
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

            // Play pop sound
            if (popAudio != null)
            {
                popAudio.Play();
            }
        }
    }
}
