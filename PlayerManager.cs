using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("---- Managers ----")]
    public GunControl gunCon;
    public WorldManager wolrdMan;
    public PowerUpManager powerMan;

    [Header("---- System ----")]
    protected bool blockMove = false;
    private float backTrackBlocker = 0f;
    private Rigidbody myRig;
    private Camera cam;

    [Header("---- Attributes -----")]
    protected float speed = 6f;
    protected float jumpSpeed = 6f;
    protected float primFireDelay = 0.5f;
    private float health = 250f;
    private float maxHealth = 250f;
    private bool invincible = false;

    [Header("---- UI ----")]
    public Image healthBar;
    public GameObject deathScreen;
    public Text endScore;
    private int layerMask;

    // jump ----
    private float jumpHeight = 2f;
    private float jumpCharge;
    private bool jumpDone = false;

    // --- andriod bools ---
    private bool isLeft = false;
    private bool isRight = false;
    private bool isJump = false;
    
    // ###################################### GETS & SETS
    private float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float JumpHeight
    {
        get { return jumpHeight; }
    }

    public bool BlockMove
    {
        get { return blockMove; }
        set { blockMove = value; }
    }

    public float PrimFireDelay
    {
        get { return primFireDelay; }
        set { primFireDelay = value; }
    }

    public bool Invinicble
    {
        set { invincible = value; }
    }

    // #################################### INI
    /// <summary>
    /// ini player in this funtion
    /// </summary>
    void Start()
    {
        jumpCharge = jumpHeight;

        myRig = GetComponent<Rigidbody>();
        cam = Camera.main;

        layerMask = LayerMask.GetMask("Default"); // set lyer mask for the shoot raycast

    }
    
    // #################################### SYSTEM
    /// <summary>
    /// Input handeld by update
    /// </summary>
    void FixedUpdate() 
    {
        if(Input.GetAxis("Horizontal") < 0 || isLeft) // left
        {
           // Debug.Log("Left pressed");
            MovePlayer(-1);
        }
        else if (Input.GetAxis("Horizontal") > 0 || isRight) // right
        {
           // Debug.Log("Right pressed");
            MovePlayer(1);
        }

        if (Input.GetAxis("Jump") > 0 || isJump) // jump
        {
           // Debug.Log("Jump press");
            JumpPlayer();
        }
        else // jump let early go
        {
            JumpCooldown();
        }

        if(Input.GetAxis("Fire") > 0) // primary fire
        {
            Debug.Log("Shoot primary");

            // ray cast check
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 30f))
            {
                //Debug.Log("Hit: " + hit.transform.gameObject.layer);
                // get point ignore z
                if (hit.transform.gameObject.layer != 5)
                    Shoot();
            }
        }

        #if UNITY_STANDALONE_WIN
        if(Input.GetAxis("Fire") < 0) // secondary click
        {
            PowerUp();
        }
        #endif
    }

    /// <summary>
    /// Move player, can upgrade faster
    /// </summary>
    /// <param name="direction"></param>
    // ##################################################### MOVEMENT
    private void MovePlayer(int direction)
    {
        if(!blockMove)
        {
            if (direction < 0)
            {
                //palyerRig.velocity = Vector3.left * speed;
                transform.Translate(Vector3.left * Time.deltaTime * speed);
                backTrackBlocker += (Vector3.left.x *Time.deltaTime * speed) * -1;
               // Debug.Log("Count up backTrackBlocker :" + backTrackBlocker);
            }
            else
            {
                //palyerRig.velocity = Vector3.right * speed;
                transform.Translate(Vector3.right * Time.deltaTime * speed);

                if (backTrackBlocker > -0.99999f) // in positives
                {
                    wolrdMan.BlockSpawn = true;
                    backTrackBlocker -= (Vector3.right.x * Time.deltaTime * speed);
                }

                if (backTrackBlocker < 0f) // in negitives
                {
                    wolrdMan.BlockSpawn = false;
                    wolrdMan.DistanceGain += (Vector3.right.x * Time.deltaTime * speed);
                }
                
            }
        }  
    }

    /// <summary>
    /// Use button input for andriod build to move
    /// </summary>
    /// <param name="direction"></param>
    public void MovePlayerAdroid(int direction)
    {
        if(direction == -1) // left
        {
            isLeft = true;
        }
        
        if(direction == 1) // right
        {
            isRight = true;
        }

        if(direction == 2) // jump
        {
            isJump = true;
        }
    }

    /// <summary>
    /// Use button input for andriod build to stop movement
    /// </summary>
    /// <param name="direction"></param>
    public void StopPlayerAdroid(int direction)
    {
        if (direction == -1) // left
        {
            isLeft = false;
        }

        if (direction == 1) // right
        {
            isRight = false;
        }

        if (direction == 2) // jump
        {
            isJump = false;
        }
    }

    /// <summary>
    /// Player jump, can upgrade to multi jump
    /// Use jump height, and dubble on powerup
    /// </summary>
    private void JumpPlayer()
    {
        if (jumpCharge > 0f)
        {
            if (!jumpDone)
            {
                jumpCharge -= Vector3.up.y * Time.deltaTime * jumpSpeed;
                transform.Translate(Vector3.up * Time.deltaTime * jumpSpeed);
                //LockRigidbody(true);
                myRig.useGravity = false;
            }
            else
            {
                myRig.useGravity = true;
                //LockRigidbody(false);
            }
        }
        else
        {
            myRig.useGravity = true;
            //LockRigidbody(false);
        }
        
    }

    /// <summary>
    /// Jump cooldown control
    /// </summary>
    private void JumpCooldown()
    {
        if(jumpCharge < jumpHeight)
        {
            myRig.useGravity = true;
            //LockRigidbody(false);
            jumpDone = true;
            jumpCharge += Vector3.up.y * Time.deltaTime * jumpSpeed;
        }
        else
        {
            jumpCharge = jumpHeight;
            jumpDone = false;
        }
    }

    /// <summary>
    /// Freeze and unfreze rigidbody
    /// </summary>
    /// <param name="status"></param>
    private void LockRigidbody(bool status)
    {
        if (status)
        {
            myRig.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            myRig.constraints = RigidbodyConstraints.None; // unfreeze all
            myRig.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        }
    }

    /// <summary>
    /// Primary shooting, can upgrade to multi shoot, rapid fire, shotgun
    /// </summary>
    // ##################################################### PLAYER ACTIONS
    private void Shoot()
    {
        gunCon.Shoot(primFireDelay);
    }
    
    /// <summary>
    /// Make power class to switch you special attacks
    /// </summary>
    /// <param name="avalible"></param>
    public void PowerUp()
    {
       powerMan.UsePowerUp();
    }

    /// <summary>
    /// Add power up IDs to player if open
    /// </summary>
    /// <param name="powerID"></param>
    private void AddPowerUp(int powerID)
    {
      //  Debug.Log("Add power up " + powerID);
        powerMan.AddPowerUp(powerID);
    }

    /// <summary>
    /// Collicion control for player
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Item") // get item affects when hit
        {
            int powerUp;
            ItemAction tempIA = col.gameObject.GetComponent<ItemAction>();
            wolrdMan.AddToScore(tempIA.Value, new Vector3(col.gameObject.transform.position.x, col.gameObject.transform.position.y, col.gameObject.transform.position.z));
            powerUp = tempIA.PowerUpID;

            Heal(tempIA.Heal);
            AddPowerUp(powerUp);

            tempIA.HitItem(); // despawn Item
        }
    }

    // ################################################### ATTRIBUTES
    /// <summary>
    /// Player take dmg
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDMG(float dmg)
    {
        if (invincible)
            dmg = 0f; // take no dmg

        health -= dmg;

        GameObject tempOb = Instantiate(wolrdMan.promp, wolrdMan.spawnContainer.transform);
        tempOb.transform.position = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z); ;

        StartCoroutine(tempOb.GetComponent<Prompt>().PromptForTime("-" + dmg.ToString(), 1f, Color.red, wolrdMan.sfxSource, wolrdMan.playerDmg));

        UpdateHealthbar();
        if (health < 0f)
        {
            // play death seq
            PlayerDeath();
        }
    }
    
    /// <summary>
    /// Heal player by amount
    /// </summary>
    /// <param name="val"></param>
    public void Heal(float val)
    {
        health += val;

        if(val > 0)
        {
            GameObject tempOb = Instantiate(wolrdMan.promp, wolrdMan.spawnContainer.transform);
            tempOb.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            StartCoroutine(tempOb.GetComponent<Prompt>().PromptForTime("+" + val.ToString(), 1f, Color.green, wolrdMan.sfxSource, wolrdMan.playerHeal));
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        UpdateHealthbar();
    }

    /// <summary>
    /// Player deah seq
    /// </summary>
    public void PlayerDeath()
    {
        blockMove = true;
        Time.timeScale = 0;
        this.GetComponent<AimControl>().AimOn = false;

        endScore.text = "Score: " + wolrdMan.HighScore;

        DeathScreen();
    }

    /// <summary>
    /// Sets jump height and charge
    /// </summary>
    /// <param name="jumpH"></param>
    public void SetJumpHeight(float jumpH)
    {
        jumpHeight = jumpH;
        jumpCharge = jumpHeight;
    }

    // ###################################### UI

    /// <summary>
    /// Update health bar UI
    /// </summary>
    private void UpdateHealthbar()
    {
        float per = health / maxHealth;

        healthBar.transform.localScale = new Vector3(per , 1f, 1f);

        if (per < 0)
        {
            healthBar.transform.localScale = new Vector3(0f, 1f, 1f);
        }

    }

    /// <summary>
    /// show death screen
    /// </summary>
    private void DeathScreen()
    {
        deathScreen.SetActive(true);
    }

    /// <summary>
    /// Restart the lvl
    /// </summary>
    public void RestartBTN()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Exit to main menu
    /// </summary>
    public void ExitToMainMneu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
