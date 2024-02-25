using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image _panel;
    [SerializeField] private FruitController _controller;

    public void LoadScene(int id) {
        SceneManager.LoadScene(id);
    }

    public void LoadSceneFromMenu() {
        AudioSource audioSource = FindAnyObjectByType<AudioSource>();
        
        if (audioSource)
            StartCoroutine(MainMenuLoadAnimation(audioSource));
    }

    public void RestartGame() {
        GameController.IsPlaying = false;
        StartCoroutine(RestartAnimation());
    }

    private IEnumerator RestartAnimation() {
        yield return StartCoroutine(_controller.DestroyAllFruitsAnimation());
        LoadScene(1);
    }

    private IEnumerator MainMenuLoadAnimation(AudioSource audioSource) {
        float t = 0;

        _panel.gameObject.SetActive(true);
        yield return new WaitUntil(() => {
            t += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(1f, 0f, t / 2f);
            _panel.color = new Color(0, 0, 0, t / 2f);

            return t >= 2f;
        });

        SceneManager.LoadScene(1);
    }
}
