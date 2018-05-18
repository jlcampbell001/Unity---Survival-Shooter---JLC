using UnityEngine;
using System.Collections;


public class EnemyShootAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
    public int attackDamage = 10;               // The amount of health taken away per attack.
    public float shootPlayerRange = 5f;         // How far can the enemy look to see / shoot the player.
    public float timeBetweenShots = 0.15f;      // The time between each shot.


    Animator anim;                              // Reference to the animator component.
    GameObject player;                          // Reference to the player GameObject.
    PlayerHealth playerHealth;                  // Reference to the player's health.
    EnemyHealth enemyHealth;                    // Reference to this enemy's health.
    bool playerToClose;                         // Whether player is within the trigger collider and can be attacked.
    float timer;                                // Timer for counting up to the next attack.
    Ray shootRay;                               // A ray from the gun end forwards.
    RaycastHit shootHit;                        // A raycast hit to get information about what was hit.
    int playerMask;                             // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem shootParticles;              // Reference to the particle system.
    LineRenderer shootLine;                     // Reference to the line renderer.
    AudioSource shootAudio;                     // Reference to the audio source.
    Light shootLight;                           // Reference to the light component.
    float effectsDisplayTime = 0.2f;            // The proportion of the timeBetweenShots that the effects will display for.

    void Awake()
    {
        // Setting up the references.
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponentInParent<EnemyHealth>();
        anim = GetComponentInParent<Animator>();

        // Create a layer mask for the Player layer.
        playerMask = LayerMask.GetMask("Player");

        shootParticles = GetComponent<ParticleSystem>();
        shootLine = GetComponent<LineRenderer>();
        shootAudio = GetComponent<AudioSource>();
        shootLight = GetComponent<Light>();

    }


    // Can not shoot the player if too close.
    void OnTriggerEnter(Collider other)
    {
        // If the entering collider is the player...
        if (other.gameObject == player)
        {
            // ... the player is in range.
            playerToClose = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        // If the exiting collider is the player...
        if (other.gameObject == player)
        {
            // ... the player is no longer in range.
            playerToClose = false;
        }
    }


    void Update()
    {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range, this enemy is alive...
        if (timer >= timeBetweenAttacks && !playerToClose && enemyHealth.currentHealth > 0 && CanShootPlayer())
        {
            // ... attack.
            Shoot();
        }

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= timeBetweenShots * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }

        // If the player has zero or less health...
        if (playerHealth.currentHealth <= 0)
        {
            // ... tell the animator the player is dead.
            anim.SetTrigger("PlayerDead");
        }
    }

    bool CanShootPlayer()
    {
        bool shootPlayer = false;

        if (playerHealth.currentHealth > 0)
        {

            shootRay.origin = new Vector3(transform.position.x, 0.0f, transform.position.z);
            shootRay.direction = transform.forward;

            if (Physics.Raycast(shootRay, out shootHit, shootPlayerRange, playerMask))
            {
                //Debug.Log("hit: " + shootHit.transform.name);
                if (shootHit.transform.tag == "Player")
                {
                    shootPlayer = true;
                }
            }
        }

        //Debug.Log(seePlayer);
        return shootPlayer;
    }

    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        shootLine.enabled = false;
        shootLight.enabled = false;
    }

    void Shoot()
    {
        if (playerHealth.currentHealth > 0)
        {
            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
            shootAudio.Play();

            // Enable the light.
            shootLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            shootParticles.Stop();
            shootParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            shootLine.enabled = true;
            shootLine.SetPosition(0, transform.position);

            // Set the second position of the line renderer to the point the raycast hit.
            shootLine.SetPosition(1, shootHit.point);

            // If the player has health to lose...
            // ... damage the player.
            playerHealth.TakeDamage(attackDamage);
        }
    }
}