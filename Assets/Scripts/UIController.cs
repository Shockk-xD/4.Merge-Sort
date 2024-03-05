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
    [SerializeField] private Text _bouncinessText;
    [SerializeField] private Text _frictionText;
    [SerializeField] private Text _spawnDelayText;
    [SerializeField] private Image[] _nextFruitsUI;
    [SerializeField] private Button _dequeueButton;
    [Header("Fun Mode Properties")]
    [SerializeField] private Text _funModeText;

    private bool _isInfinityBuffs = false;

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
        if (_isInfinityBuffs) {
            _cleanButtonCountText.text = "∞";
            return;
        }

        _cleanButtonCountText.text = GameController.instance.cleanCount.ToString();
    }

    public void DequeueButtonClick() {
        FruitController.instance.Dequeue();
    }

    public void UpdateDequeueButtonText() {
        if (_isInfinityBuffs) {
            _dequeueButtonCountText.text = "∞";
            return;
        }

        _dequeueButtonCountText.text = GameController.instance.dequeueCount.ToString();
    }

    public void DequeueButtonInteractable(bool value) {
        _dequeueButton.interactable = value;
    }

    public void UpdateBouncinessText(float value) {
        _bouncinessText.text = value.ToString("0.0");
    }

    public void UpdateFrictionText(float value) {
        _frictionText.text = value.ToString("0.0");
    }

    public void UpdateSpawnDelayText(float delay) {
        _spawnDelayText.text = delay.ToString("0.0");
    }

    public void FunModeButtonClick() {
        canvasAnimator.SetTrigger("Goto Fun Settings");
        
        if (!GameController.IsFunMode) {
            GameController.instance.EnableFunMode();
            _funModeText.text = "Edit" + "\n" + "Fun Mode";
        }
    }

    public void FunSettingsCloseButtonClick() {
        GameController.instance.Resume("Fun Settings Close");
    }

    public void SetInfinityBuffsButtonClick(bool isInfinity) {
        _isInfinityBuffs = isInfinity;
        UpdateCleanButtonText();
        UpdateDequeueButtonText();
    }
}
