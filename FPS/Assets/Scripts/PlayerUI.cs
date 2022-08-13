using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform thrusterFuelFill;

    [SerializeField]
    private RectTransform healthBarFill;

    private PlayerController controller;
    private Player player;
    private WeaponManager manager;

    [SerializeField]
    private GameObject scoreBoard;

    [SerializeField]
    private TMP_Text healthPoint;

    [SerializeField]
    private TMP_Text ammoText;

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();   
        manager = player.GetComponent<WeaponManager>();
    }

    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(amount, 1f, 1f);
    }

    void SetHealthBarAmount(float amount)
    {
        healthBarFill.localScale = new Vector3(amount, 1f, 1f);
        healthPoint.text = Mathf.Round(player.GetHealthPct() * 100).ToString();
    }

    void SetAmmoAmount(int currentAmmo)
    {
        Debug.Log("HERE");
        ammoText.text = currentAmmo + "/" + manager.GetCurrentWeapon().magazineSize ;
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());

        SetHealthBarAmount(player.GetHealthPct());

        SetAmmoAmount(manager.currentMagazineSize);

        if (Input.GetKeyDown(KeyCode.Tab) ) 
        {
            scoreBoard.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }
}
