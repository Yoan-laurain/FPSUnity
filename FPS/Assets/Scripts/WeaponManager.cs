using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    private WeaponGraphics currentGraphics;

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        //On crée l'arme et on la positionne au niveau du weapon holder
        GameObject weapon = Instantiate(_weapon.graphics, weaponHolder.position,weaponHolder.rotation);

        //On met l'arme en tant que enfant du weapon holder pour qu'elle suive le mouvement
        weapon.transform.SetParent(weaponHolder);

        if(isLocalPlayer)
        {
            Util.SetLayerRecursively(weapon, LayerMask.NameToLayer(weaponLayerName));
        }

        currentGraphics = weapon.GetComponent<WeaponGraphics>();
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }


}
