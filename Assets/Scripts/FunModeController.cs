using UnityEngine;
using UnityEngine.UI;

public class FunModeController : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D _material;
    [SerializeField] private UIController _controllerUI;
    [SerializeField] private Slider _minFruitSlider;
    [SerializeField] private Slider _maxFruitSlider;
    [SerializeField] private Image _minFruitIcon;
    [SerializeField] private Image _maxFruitIcon;

    private void Awake() {
        _material.bounciness = 0.3f;
        _material.friction = 0.3f;
    }

    public void ChangeBounciness(float value) {
        _material.bounciness = value;
        _controllerUI.UpdateBouncinessText(value);
    }

    public void ChangeFriction(float value) {
        _material.friction = value;
        _controllerUI.UpdateFrictionText(value);
    }

    public void ChangeSpawnDelay(float delay) {
        FruitController.spawnDelay = delay;
        _controllerUI.UpdateSpawnDelayText(delay);
    }

    public void ChangeMinFruit(float index) {
        int newMinFruitIndex = Mathf.Clamp((int)index, 0, FruitController.maxFruitIndex);
        _minFruitIcon.sprite = _controllerUI.fruitSprites[newMinFruitIndex];
        _minFruitSlider.value = newMinFruitIndex;
        FruitController.minFruitIndex = newMinFruitIndex;
    }
    
    public void ChangeMaxFruit(float index) {
        int newMaxFruitIndex = Mathf.Clamp((int)index, FruitController.minFruitIndex, 10);
        _maxFruitIcon.sprite = _controllerUI.fruitSprites[newMaxFruitIndex];
        _maxFruitSlider.value = newMaxFruitIndex;
        FruitController.maxFruitIndex = newMaxFruitIndex;
    }
}
