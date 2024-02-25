using UnityEngine;

public class UIMethods : MonoBehaviour
{
    [SerializeField] private GameObject _trait;

    public void DisableTraitOnTrue(bool value) {
        _trait.SetActive(!value);
    }
}
