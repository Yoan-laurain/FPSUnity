using UnityEngine;
using Mirror;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private WeaponData primaryWeapon;

    private WeaponData currentWeapon;

    [HideInInspector]
    public int currentMagazineSize;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    private WeaponGraphics currentGraphics;

    [HideInInspector]
    public bool isReloading = false;

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public void EquipWeapon(WeaponData _weapon)
    {
        currentWeapon = _weapon;
        currentMagazineSize = _weapon.magazineSize;

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

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public IEnumerator Reload()
    {
        if (!isReloading)
        {
            isReloading = true;

            CmdOnReload();
            yield return new WaitForSeconds(currentWeapon.reloadTime);
            currentMagazineSize = currentWeapon.magazineSize;

            isReloading = false;
        }
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator animator = currentGraphics.GetComponent<Animator>();   
        if(animator != null )
        {
            animator.SetTrigger("Reload");
        }

        AudioSource sound = GetComponent<AudioSource>();
        sound.PlayOneShot(currentWeapon.reloadSound);

    }


}
