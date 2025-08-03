using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject LoadingScree;
    public Slider ProgressBar;
    public TMP_Text ProgressText;
    public string SceneToLoad = "MainScene";

    private AsyncOperation asyncOperation;

    void Awake()
    {
        if (LoadingScree) LoadingScree.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if (LoadingScree) LoadingScree.SetActive(true);
        SceneToLoad = sceneName;
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync(SceneToLoad);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            ProgressBar.value = progress;
            ProgressText.text = ((float)progress * 100) + "%";
            yield return null;
        }
    }
}
