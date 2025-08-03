using System.Collections.Generic;
using System.Linq;
using BotRoot;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BetaTestGameManager : MonoBehaviour
{
    public LayerMask botMask;
    [SerializeField] private List<BotAuthor> authors;
    [SerializeField] private GameObject WinScreen;

    private new PlayerAudio audio;
    private bool gameEnded = false;
    void Awake()
    {
        audio = FindAnyObjectByType<PlayerAudio>();
        RelistBot();
        InvokeRepeating(nameof(CheckEndGame), 1, 1);
    }

    public void CheckEndGame()
    {
        if(gameEnded) return;
        if (authors?.All(author => author?.setup?.health?.died ?? false) == true)
        {
            WinScreen?.SetActive(true);
            audio?.audio.PlayOneShot(audio.MUSIC_COMPLETED_THEME);
            gameEnded = true;
        }
    }
    public void RelistBot()
    {
        authors = new();
        Collider[] botsCollider = Physics.OverlapSphere(transform.position, 200, botMask);
        foreach (Collider collider in botsCollider)
        {
            if (collider.TryGetComponent(out BotAuthor author))
            {
                if (author.setup.health.died) continue;
                authors.Add(author);
            }
        }
    }

    public void ResetGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void BackToMenu() => SceneManager.LoadScene("MainMenu");
}
