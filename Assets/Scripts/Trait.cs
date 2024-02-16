using UnityEngine;

public class Trait : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == 0 && GameController.IsPlaying)
            GameController.instance.Lose();
    }
}
