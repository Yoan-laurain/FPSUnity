using Mirror;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    private void Start()
    {
        // Si c'est pas notre joueur
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();    
        }
        else
        {
            sceneCamera = Camera.main;

            //On désactive la camera de pre-spawn
            sceneCamera.gameObject.SetActive(false);
        }

        GetComponent<Player>().SetUp();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netID, player);
    }

    private void AssignRemoteLayer()
    {
        // On donne un tag au autre joueur
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    private void DisableComponents()
    {
        // On désactive tout les composants qui ne nous appartient pas
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        if(sceneCamera != null)
        {
            //On réactive la camera de pre-spawn
            sceneCamera.gameObject.SetActive(true);
        }

        //Retire le joueur de la liste des joueurs
        GameManager.UnRegistrerPlayer(transform.name);
    }
}
