using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

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
    }

    [Client]
    private void Shoot()
    {
        //Ne pas tirer pour les autres joueurs
        if (!isLocalPlayer) return;

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
                CmdPlayerShoot(hit.collider.name);
            }

            CmdOnHit(hit.point,hit.normal);

        }

    }

    //Grâce à command toutes les infos dedans sont envoyés au serveur et partagé dans toutes les instances
    [Command]
    private void CmdPlayerShoot(string playerId)
    {
        Debug.Log(playerId + " a été touché");

        Player player = GameManager.GetPlayer(playerId);

        player.RpcTakeDamage(currentWeapon.damage);
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
