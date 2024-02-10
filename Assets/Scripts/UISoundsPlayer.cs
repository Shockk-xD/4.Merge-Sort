using UnityEngine;

public class UISoundsPlayer : MonoBehaviour
{
    public void PlaySound(string soundName) {
        switch (soundName) {
            case "ShowUI":
                AudioController.instance.PlaySound(AudioController.Sound.ShowUI);
                break;
            case "BoardFull":
                AudioController.instance.PlaySound(AudioController.Sound.BoardFull);
                break;
        }
    }
}
