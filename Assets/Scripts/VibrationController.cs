using UnityEngine;
using CandyCoded.HapticFeedback;

public class VibrationController : MonoBehaviour
{
    public static void Vibrate() {
        if (GameController.IsPlaying)
            HapticFeedback.MediumFeedback();
    }
}
