using System.Collections;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField]
    private WeaponData theWeapon;

    private GameObject pickUpGraphics;
    private bool canPickUp;
    private float respawnDelay = 20f;


    private void Start()
    {
        ResetWeapon();
    }

    void ResetWeapon()
    {
        pickUpGraphics = Instantiate(theWeapon.graphics,transform);
        pickUpGraphics.transform.position = transform.position;

        canPickUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Si un joueur passe au dessus de l'arme
        if(other.CompareTag("Player") && canPickUp)
        {
            WeaponManager weaponManager = other.GetComponent<WeaponManager>();
            EquipNewWeapon(weaponManager);
        }
    }

    void EquipNewWeapon(WeaponManager weaponManager)
    {
        //On détruit l'arme actuelle du joueur
        Destroy(weaponManager.GetCurrentGraphics().gameObject);

        //On equipe la nouvelle arme
        weaponManager.EquipWeapon(theWeapon);

        canPickUp = false;

        //On détruit l'arme ramassée
        Destroy(pickUpGraphics);

        StartCoroutine(DelayResetWeapon());
    }

    IEnumerator DelayResetWeapon()
    {
        // Temps avant de faire ré apparaitre l'arme
        yield return new WaitForSeconds(respawnDelay);
        ResetWeapon();
    }
}
