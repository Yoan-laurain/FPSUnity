using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetUp))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _idDead = false ;

    public bool isDead
    {     
      get { return _idDead; }
      protected set { _idDead = value; }
    }

    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar]
    private float currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    private bool[] wasEnabledOnStart;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    public int kills;
    public int death;

    [SerializeField]
    private AudioClip hitSound;

    [SerializeField]
    private AudioClip destroySound;


    public void SetUp()
    {
        if(isLocalPlayer)
        {
            //Changement de camera

            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetUp>().playerUIInstance.SetActive(true);

        }

        RpcSetupPlayerOnAllClients();
    }

    [Command(requiresAuthority = false)]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabledOnStart = new bool[disableOnDeath.Length];

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabledOnStart[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    private IEnumerator Respawn()
    {
        //Delais de respawn
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);

        //Point de spawn
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetUp();
    }

    public void SetDefaults()
    {
        //Vie de base
        currentHealth = maxHealth;

        isDead = false;

        //On r�active les components du joueur
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        //On r�active le collider du joueur
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }

        //On r�active le visuel du joueur
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        //Apparition du syst�me de particules lors du spawn
        GameObject gfx = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(gfx, 3f);
    }

    [ClientRpc]
    public void RpcTakeDamage(float damage, string sourceID)
    {
        //Si il est d�j� mort 
        if(isDead)
        {
            return;
        }

        AudioSource sound = GetComponent<AudioSource>();

        //On soustrait les pv
        currentHealth -= damage;

        //Si il � plus de vie
        if(currentHealth <= 0)
        {
            Die(sourceID);
            sound.PlayOneShot(destroySound);
        }

        sound.PlayOneShot(hitSound);
    }

    private void Die(string sourceID)
    {
        isDead = true;

        // On r�cup�re celui qui � tu� le joueur
        Player sourcePlayer = GameManager.GetPlayer(sourceID);

        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallBack.Invoke(transform.name,sourceID);
        }

        death++;

        // D�sactive les components du joueur lors de la mort
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // D�sactive les components visuel du joueur lors de la mort
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }

        // D�sactive le collider du joueur
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        //Apparition du syst�me de particules lors de la mort
        GameObject gfx = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gfx, 3f);

        //Changement de camera
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetUp>().playerUIInstance.SetActive(false);
        }

        StartCoroutine(Respawn());
    }

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }
}
