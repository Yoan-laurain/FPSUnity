using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    public PlayerWeapon weapon;

    [SerializeField]
    private LayerMask mask;

    private void Update()
    {
        // Si clic gauche
        if( Input.GetButtonDown("Fire1") )
        {
            Shoot();
        }
    }

    [Client]
    private void Shoot()
    {
        // Cible touché
        RaycastHit hit;

        // Param : 1 -> Point de départ du tir, 2 -> Direction, 3 -> Ou on stocke la cible, 4 -> longueur du tir, Cible qui nous intéresse
        if( Physics.Raycast( cam.transform.position,cam.transform.forward, out hit, weapon.range, mask ) ) 
        {
            //On vérifie que c'est bien un joueur qui est touché
            if(hit.collider.tag == "Player")
            {
                //On affiche son nom dans la console
                CmdPlayerShoot(hit.collider.name);
            }

        }

    }

    //Grâce à command toutes les infos dedans sont envoyés au serveur et partagé dans toutes les instances
    [Command]
    private void CmdPlayerShoot(string playerId)
    {
        Debug.Log(playerId + " a été touché");

        Player player = GameManager.GetPlayer(playerId);

        player.RpcTakeDamage(weapon.damage);
    }

}
