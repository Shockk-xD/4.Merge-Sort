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
    [SerializeField] private Image[] _nextFruitsUI;
    [SerializeField] private Button _undoButton;

    private void Start() {
        int score = GameController.instance.Score;
        int bestScore = PlayerPrefs.GetInt("Best Score");

        UpdateScoreText(score);
        UpdateBestScoreText(bestScore);
        UpdateCleanButtonText();
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

    public void UndoButtonClick() {
        SaveData.instance.LoadData();
    }

    public void EnableUndoButton() {
        _undoButton.interactable = true;
    }
}
