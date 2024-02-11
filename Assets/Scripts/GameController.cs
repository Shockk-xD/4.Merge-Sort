using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    [SerializeField] private UIController _controllerUI;
    [SerializeField] private Animator _animatorUI;

    public static GameController instance;
    public static bool IsPlaying { get; private set; }
    public int Score {
        get {
            return _score;
        }
        set {
            if (!IsPlaying) return;
            _score = value;
            _controllerUI.UpdateScoreText(_score);

            if (_score > _bestScore) {
                _bestScore = _score;
                _controllerUI.UpdateBestScoreText(_bestScore);
            }
        }
    }

    public int cleanCount = 3;
    public int dequeueCount = 5;

    private int _score = 0;
    private int _bestScore;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        IsPlaying = true;

        _bestScore = PlayerPrefs.GetInt("Best Score", 0);
    }

    public void Lose() {
        _animatorUI.SetTrigger("Lose");
        IsPlaying = false;
        _controllerUI.UpdateLoseMenuScoreText(_score);
    }

    public void BackToMenu() {
        _animatorUI.SetTrigger("Back Menu");
        IsPlaying = false;
    }

    public void Resume() {
        _animatorUI.SetTrigger("Back Menu Close");
        StartCoroutine(ChangeBoolAfterDelay());
    }

    private IEnumerator ChangeBoolAfterDelay() {
        yield return new WaitForSeconds(0.5f);
        IsPlaying = true;
    }

    private void OnDestroy() {
        PlayerPrefs.SetInt("Best Score", _bestScore);
        PlayerPrefs.Save();
    }
}
