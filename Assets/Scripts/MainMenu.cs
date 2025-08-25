using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject fadePanel;   // panel with Image (full screen)

    private Image fadeImage;

    private void Start()
    {
        if (fadePanel != null)
        {
            fadeImage = fadePanel.GetComponent<Image>();
            fadePanel.SetActive(false);
        }
    }

    // 🔹 Call this in the inspector OnClick, pass the scene name string
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndExecute(() =>
        {
            SceneManager.LoadScene(sceneName);
        }));
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }

    public void QuitGame()
    {
        StartCoroutine(FadeAndExecute(() =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }));
    }

    private IEnumerator FadeAndExecute(System.Action action)
    {
        if (fadePanel != null && fadeImage != null)
        {
            fadePanel.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;

            float t = 0;
            while (t < 2f) // 2 seconds fade
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0, 1, t / 2f);
                fadeImage.color = c;
                yield return null;
            }
        }

        action?.Invoke();
    }
}
