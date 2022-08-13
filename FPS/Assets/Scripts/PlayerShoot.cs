using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private WeaponData currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        //Reload
        if(Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize < currentWeapon.magazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        //Coup par coup
        if(currentWeapon.fireRate <= 0f)
        {
            // Si clic gauche
            if( Input.GetButtonDown("Fire1") )
            {
                Shoot();
            }

        }
        // Tir automatique
        else
        {           
            // Si clic gauche
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1") ) 
            {
                CancelInvoke("Shoot");
            }

        }

    }

    // Fonction appelée sur le serveur lorsque notre joueur tir ( on prévient le serveur de notre tir )
    [Command]
    void CmdOnShoot()
    {
        RpcToShootEffects();
    }

    // Fait apparaitre les effets de tir chez tous les clients / joueurs
    [ClientRpc]
    void RpcToShootEffects()
    {
        //On joue le système de particules
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();

        // On joue le son du tir

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.shootSound);
    }

    [Client]
    private void Shoot()
    {
        //Ne pas tirer pour les autres joueurs ou si on recharge
        if (!isLocalPlayer || weaponManager.isReloading) return;

        //Si on a plus de balles
        if (weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        weaponManager.currentMagazineSize--;

        CmdOnShoot();

        // Cible touché
        RaycastHit hit;

        // Param : 1 -> Point de départ du tir, 2 -> Direction, 3 -> Ou on stocke la cible, 4 -> longueur du tir, Cible qui nous intéresse
        if( Physics.Raycast( cam.transform.position,cam.transform.forward, out hit, currentWeapon.range, mask ) ) 
        {
            //On vérifie que c'est bien un joueur qui est touché
            if(hit.collider.tag == "Player")
            {
                //On affiche son nom dans la console
                CmdPlayerShoot(hit.collider.name,currentWeapon.damage,transform.name);
            }

            CmdOnHit(hit.point,hit.normal);

        }

        if (weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

    }

    //Grâce à command toutes les infos dedans sont envoyés au serveur et partagé dans toutes les instances
    [Command]
    private void CmdPlayerShoot(string playerId,float damage, string sourceID)
    {
        Player player = GameManager.GetPlayer(playerId);

        player.RpcTakeDamage(damage, sourceID);
    }

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcToHitEffects(pos,normal);
    }

    [ClientRpc]
    void RpcToHitEffects(Vector3 pos, Vector3 normal)
    {
        //Onjoue la particule
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));

        //On la detruit de la hiérarchie
        Destroy(hitEffect, 1f);
    }
}
