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

        //On réactive les components du joueur
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        //On réactive le collider du joueur
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }

        //On réactive le visuel du joueur
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        //Apparition du système de particules lors du spawn
        GameObject gfx = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(gfx, 3f);
    }

    [ClientRpc]
    public void RpcTakeDamage(float damage)
    {
        //Si il est déjà mort 
        if(isDead)
        {
            return;
        }

        //On soustrait les pv
        currentHealth -= damage;

        //Si il à plus de vie
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Désactive les components du joueur lors de la mort
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Désactive les components visuel du joueur lors de la mort
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }

        // Désactive le collider du joueur
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        //Apparition du système de particules lors de la mort
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
}
