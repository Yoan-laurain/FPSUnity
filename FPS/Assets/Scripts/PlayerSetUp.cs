using Mirror;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    [SerializeField]
    private string dontDrawLayerName = "DontDraw";

    [SerializeField]
    private GameObject playerGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;

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
            // On applique le layher pour ne pas afficher notre perso à notre camera

            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName) );

            //Player UI
            playerUIInstance = Instantiate(playerUIPrefab);

            // Configuration du UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();

            if(ui != null)
            {
                ui.SetController(GetComponent<PlayerController>());
            }

            GetComponent<Player>().SetUp();
        }

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
        Destroy(playerUIInstance);

        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }

        //Retire le joueur de la liste des joueurs
        GameManager.UnRegistrerPlayer(transform.name);
    }
}
