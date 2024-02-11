using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Animator canvasAnimator;
    public Sprite[] fruitSprites;
    public List<Sprite> fruitsQueueUI = new List<Sprite>(3);

    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _bestScoreText;
    [SerializeField] private Text _loseMenuScoreText;
    [SerializeField] private Text _cleanButtonCountText;
    [SerializeField] private Text _dequeueButtonCountText;
    [SerializeField] private Image[] _nextFruitsUI;
    [SerializeField] private Button _dequeueButton;

    private void Start() {
        int score = GameController.instance.Score;
        int bestScore = PlayerPrefs.GetInt("Best Score");

        UpdateScoreText(score);
        UpdateBestScoreText(bestScore);
        UpdateCleanButtonText();
        UpdateDequeueButtonText();
    }

    public void UpdateUIQueue() {
        for (int i = 0; i < _nextFruitsUI.Length; i++) {
            _nextFruitsUI[i].sprite = fruitsQueueUI[i];
        }
        StartCoroutine(NextFruitsAnimator());
    }

    private IEnumerator NextFruitsAnimator() {
        float t = 0;


        yield return new WaitUntil(() => {
            t += Time.deltaTime;
            for (int i = 0; i < _nextFruitsUI.Length; i++)
                _nextFruitsUI[i].rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t * 5);

            return t * 5 >= 1;
        });
    }

    public void UpdateScoreText(int score) {
        _scoreText.text = "Score" + "\n" + $"<color=red>{score}</color>";
    }

    public void UpdateBestScoreText(int score) {
        _bestScoreText.text = "Best score" + "\n" + $"<color=green>{score}</color>";
    }

    public void UpdateLoseMenuScoreText(int score) {
        _loseMenuScoreText.text = "Score" + "\n" + score;
    }

    public void UpdateCleanButtonText() {
        _cleanButtonCountText.text = GameController.instance.cleanCount.ToString();
    }

    public void DequeueButtonClick() {
        FruitController.instance.Dequeue();
    }

    public void UpdateDequeueButtonText() {
        _dequeueButtonCountText.text = GameController.instance.dequeueCount.ToString();
    }

    public void DequeueButtonInteractable(bool value) {
        _dequeueButton.interactable = value;
    }

    public IEnumerator DequeueAnimation() {
        DequeueButtonInteractable(false);

        float timer = 0;

        yield return new WaitUntil(() => {
            timer += Time.deltaTime * 4;

            _nextFruitsUI[0].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timer);

            return timer >= 1f;
        });
    }
}
