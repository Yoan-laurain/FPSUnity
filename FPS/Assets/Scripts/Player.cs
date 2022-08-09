using UnityEngine;
using Mirror;
using System.Collections;

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

    private bool[] wasEnabledOnStart;


    public void SetUp()
    {
        wasEnabledOnStart = new bool[disableOnDeath.Length];

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnabledOnStart[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    private IEnumerator Respawn()
    {
        //Delais de respawn
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);

        //On reset ces valeurs
        SetDefaults();

        //Point de spawn
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    public void SetDefaults()
    {
        //Vie de base
        currentHealth = maxHealth;

        isDead = false;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }
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

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        StartCoroutine(Respawn());
    }
}
