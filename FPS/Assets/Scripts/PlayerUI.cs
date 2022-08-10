using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform thrusterFuelFill;

    private PlayerController controller;

    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(amount, 1f, 1f);
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
    }
}
