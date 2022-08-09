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
        // Cible touch�
        RaycastHit hit;

        // Param : 1 -> Point de d�part du tir, 2 -> Direction, 3 -> Ou on stocke la cible, 4 -> longueur du tir, Cible qui nous int�resse
        if( Physics.Raycast( cam.transform.position,cam.transform.forward, out hit, weapon.range, mask ) ) 
        {
            //On v�rifie que c'est bien un joueur qui est touch�
            if(hit.collider.tag == "Player")
            {
                //On affiche son nom dans la console
                CmdPlayerShoot(hit.collider.name);
            }

        }

    }

    //Gr�ce � command toutes les infos dedans sont envoy�s au serveur et partag� dans toutes les instances
    [Command]
    private void CmdPlayerShoot(string playerId)
    {
        Debug.Log(playerId + " a �t� touch�");

        Player player = GameManager.GetPlayer(playerId);

        player.RpcTakeDamage(weapon.damage);
    }

}
