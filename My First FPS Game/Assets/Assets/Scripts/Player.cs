using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;


    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;




    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            // swtich cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }


        SetDefaults();
    }

    /*
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(9999);
        }
    }
    */

    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (isDead)
            return;
        currentHealth -= amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // disable gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // disable the collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // spawn death effect
        GameObject gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);

        // swtich cameras
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }


        Debug.Log(transform.name + " is dead");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        // enable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // enable gameobjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // enable collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        

        // create spawn effect
        GameObject gfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);
    }
}
