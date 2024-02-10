using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip _dropSound;
    [SerializeField] private AudioClip _mergeSound;
    [SerializeField] private AudioClip _successSound;
    [SerializeField] private AudioClip _showUISound;
    [SerializeField] private AudioClip _boardFullSound;
    [SerializeField] private AudioClip _buttonClickSound;

    private AudioSource _audioSource;

    public static AudioController instance;

    public enum Sound {
        Drop,
        Merge,
        Success,
        ShowUI,
        BoardFull,
        ButtonClick
    }

    private void Start() {
        instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound sound) {
        if (!(GameController.IsPlaying || sound == Sound.ShowUI || sound == Sound.BoardFull)) return;

        switch (sound) {
            case Sound.Drop:
                _audioSource.PlayOneShot(_dropSound);
                break;
            case Sound.Merge:
                _audioSource.PlayOneShot(_mergeSound);
                break;
            case Sound.Success:
                _audioSource.PlayOneShot(_successSound); 
                break;
            case Sound.ShowUI:
                _audioSource.PlayOneShot(_showUISound);
                break;
            case Sound.BoardFull:
                _audioSource.PlayOneShot(_boardFullSound);
                break;
            case Sound.ButtonClick:
                _audioSource.PlayOneShot(_buttonClickSound);
                break;
        }
    }
}
