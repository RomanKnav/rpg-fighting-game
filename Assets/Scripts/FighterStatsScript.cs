using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FighterStatsScript : MonoBehaviour
{
    [SerializeField]
    private Animator animator;          // how's this assigned? manually

    [SerializeField]

    // change this when new character selected:

    // THIS IS 'ENEMYHEALTHFILL', manually assigned in Editor.
    private GameObject healthFill;

    [SerializeField]
    private GameObject magicFill;

    private string name;                // for use in other scripts

    [Header("Stats")]
    public float health;                // what's this? a manually entered NUMBER. Does it update automatically?
    public float magic;

    public float melee;
    public float magicRange;
    public float defense;

    private float startHealth;                  // and this? the initial value of "health"
    private float startMagic;

    [HideInInspector]

    public bool dead = false;

    public Transform healthTransform;          // this is the Transform component of the HealthFill object.
    private Transform magicTransform;

    public Vector2 healthScale;                // what's this? a 2-value tuple. Used to change the size of healthbar
    private Vector2 magicScale;

    public float xNewHealthScale;
    private float xNewMagicScale;

    private GameObject gameControllerObj;    

    // this is supposed to be a script:
    private GameController gameControllerScript;

    // MY CRAP:
    [Header("MY CRAP")]
    public Sprite thumbnail;           
    public bool victim = false;     
    public float agility;               
    public bool isFriendly;
    public Sprite deadSprite;
    public Sprite currentSprite; 
    public Transform circleOutlineGreen;
    public Transform circleOutlineYellow;
    public Transform circleOutlineRed;
    public Transform currentCircleOutline;
    public Transform highlightCursor;
    public GameObject ownerObject;              // can I get the sprite's coords frpom this? Yes, we use the TRANSFORM.
    public FighterAction playerActionScript;    // how's this assigned?
    public float damageTaken = 0f;

    // is this var only ever used internally? YES
    public bool selected;
    public bool hoveringOver = false;      
    public bool drawTheCircle;              // should be true when current character's turn
    public bool turnIsOver;

    // move towards enemy when this is true. As soon as false, BAD.
    public bool turnInProgress;             // could use this to determine when to draw circle   --SUCCESS

    public bool isSniper;                   // so that exclusively range enemies dont move towards opponent

    public bool attacking = false;          // turns false as soon as character starts returning. Turns true when? as soon as we move towards enemy
    public bool retreating = false;

    // position where player stands before moving to enemy. Should be IMMUTABLE:
    public Vector3 originalPosition;       

    void Awake()
    {
        ownerObject = this.gameObject;

        name = this.name;

        if (ownerObject != null) {
            originalPosition = ownerObject.transform.position;
        }

        // HEALTH SETTING CRAP:
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;
        startHealth = health;

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startMagic = magic;

        gameControllerObj = GameObject.Find("GameControllerObject");

        if (gameControllerObj != null) {
          gameControllerScript = gameControllerObj.GetComponent<GameController>();
        }

        // MY STUFF:
        // this sprite is CONSTANTLY changing due to animations:
        currentSprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        animator = gameObject.GetComponent<Animator>();

        circleOutlineGreen = transform.GetChild(2);
        circleOutlineYellow = transform.GetChild(3);
        circleOutlineRed = transform.GetChild(4);

        highlightCursor = transform.GetChild(5);
    }

    // run once or for every character? EVERY CHARACTER, once
    void Start() {
        if (playerActionScript == null) {
            playerActionScript = gameControllerScript.currentHeroObj.GetComponent<FighterAction>();
        }
    }

    void Update() {
        SetActionScript();

        // moving this from DrawCircle() to here fixed Escape issue:
        if (Input.GetKeyDown(KeyCode.Escape) && selected == true && gameControllerScript.freeState == true) 
        {
            gameControllerScript.aCharacterLockedIn = false;

            selected = false;
            this.highlightCursor.gameObject.SetActive(false);

            // NEW ADDITION:
            gameControllerScript.cursorAlreadyActive = false; 
            gameControllerScript.characterManuallySelected = false;
        }

        if (turnInProgress == true) {
            DrawCircle();    
        } 
        else if (currentCircleOutline != null) {
            this.currentCircleOutline.gameObject.SetActive(false);
        }
    }

    void SetActionScript() {
        // ONLY BEING SET FOR FIRST HERO (must set elsewhere):
        // it works, but seems like a bad place to put this in:
        if (isFriendly == true) {
            playerActionScript = gameObject.GetComponent<FighterAction>();
        } 
        else {
            // set this way if current character is enemy:
            playerActionScript = gameControllerScript.currentHeroObj.GetComponent<FighterAction>();
        }
    }

    // used in Update():
    void DrawCircle() {
        // this sucks:
        if (health >= 70) {
            currentCircleOutline = circleOutlineGreen;
        } 
        else if (health >= 40 && health <= 69) {
            currentCircleOutline = circleOutlineYellow;
        }
        else {
            currentCircleOutline = circleOutlineRed;
        }

        if (drawTheCircle) {
            this.currentCircleOutline.gameObject.SetActive(true);
        } 
        else {
            this.currentCircleOutline.gameObject.SetActive(false);
        }
    }

    // used in OnMouseOver():
    void UpdateHealth()
    {
        xNewHealthScale = healthScale.x * (health / startHealth);

        // x size changes based on the health:
        healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
    }

    // this is for the VICTIM:
    // HERE'S WHAT UPDATES THE HEALTHBAR. Used in FighterAction.cs:
    public void ReceiveDamage(float damage)
    {
        damageTaken = damage;
        health = health - damage;

        animator.Play("Damage");

        if(health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            gameControllerScript.selectedCharacter = null; // successfully makes it empty

            animator.enabled = false;
            selected = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
            highlightCursor.gameObject.SetActive(false);
            gameControllerScript.cursorAlreadyActive = false; 

            gameControllerScript.characterManuallySelected = false;
            gameControllerScript.aCharacterLockedIn = false;

            // REMOVE DEAD CHARACTER FROM CHARACTERLIST AND PRIORITYLIST:
            gameControllerScript.charactersList.Remove(gameObject);

            gameControllerScript.priorityList.Remove(gameObject);

        // UPDATE HEALTH HERE:
        } 
        else if (damage > 0)
        {
            // what is healthScale?
            xNewHealthScale = healthScale.x * (health / startHealth);

            // x size changes based on the health:
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }
        if (damage > 0)
        {
            gameControllerScript.battleText.gameObject.SetActive(true);
            gameControllerScript.battleText.text = damage.ToString();
        }
        Invoke("ContinueGame", 2);
    }

    // REDUCE the magic fill:
    public void updateMagicFill(float cost)
    {
        if(cost > 0)
        {
            magic = magic - cost;
            xNewMagicScale = magicScale.x * (magic / startMagic);
            magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
        }
    }

    public bool GetDead()
    {
        return dead;
    }

    // WTF IS THIS????? sus. Goes to next turn when current turn over.
    // used here and in AttackScript:
    void ContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }

    public void SetEnemyThumbnail()
    {
        GameObject oppFaceObject = GameObject.Find("EnemyFace");

        if (oppFaceObject != null)
        {
            oppFaceObject.GetComponent<Image>().sprite = thumbnail;
        }
    }

    // ENEMY HEALTH SHOULD CHANGE ON HOVER:
    // FUCK. NO WORK DONE TODAY
    void OnMouseOver()
    {
        hoveringOver = true; 

        if (gameControllerScript.freeState == true) {
            if (!gameControllerScript.cursorAlreadyActive) {
                if (hoveringOver == true && !dead) {
                    this.highlightCursor.gameObject.SetActive(true);
                    gameControllerScript.cursorAlreadyActive = true;
                }

                // if another character isn't already selected:
                if (gameControllerScript.selectedCharacter != null && !dead)
                {
                    // forgot what I was doing here???
                    selected = false;
                    this.highlightCursor.gameObject.SetActive(true);
                    gameControllerScript.cursorAlreadyActive = true;

                    SetEnemyThumbnail();
                    SetEnemyName();

                    UpdateHealth();
                }
            }
        }
    }

    // why not working anymore?
    void SetEnemyName() {
       GameObject oppNameObject = GameObject.Find("EnemyFrameLabel"); 
       oppNameObject.GetComponent<Text>().text = gameObject.name;
    }

    // with 4 characters, all of this SUCKS:
    // THE BUGGER IS HERE:
    void OnMouseExit()
    {
        hoveringOver = false;

        // this only works when no character's manually selected:
        // if the current character is NOT selected (all of them initially):
        if (!selected && !gameControllerScript.characterManuallySelected)
        {
            this.highlightCursor.gameObject.SetActive(false);

            // NEW ADDITION: 
            gameControllerScript.cursorAlreadyActive = false;   
        }
    }

    // only runs when object clicked on?
    void OnMouseDown()
    {
        // CAN CONFIRM THIS NOT BEING UPDATED:
        if (!gameControllerScript.aCharacterLockedIn) 
        {
            SelectNewCharacter();
        }
    }

    // used ONLY when player manually selects new character:
    public void SelectNewCharacter() {

        gameControllerScript.aCharacterLockedIn = true;

        // if (gameControllerScript.freeState == true && ) 
        if (gameControllerScript.freeState == true) 
        {
            gameControllerScript.characterManuallySelected = true;      // why's this here?
            if ((gameControllerScript.selectedCharacter != null && !dead) || (hoveringOver == true && !dead))
            {
                selected = true;
                gameControllerScript.selectedCharacter = ownerObject;
                gameControllerScript.actionMenu.SetActive(true);

                // makes it so that next enemy to attack is the one selected:
                
                // ISSUE IS NOT IN HERE (does its job):
                if (ownerObject == null) {
                    Debug.Log("SELECTNEWCHARACTER could not find ownerObject");
                } else {
                    // this prints the VICTIM'S name:
                    Debug.Log($"ownerObject found by {gameObject.name}'s SELECTNEWCHARACTER");

                    // why's this not working to set friendly guy?:
                    playerActionScript.enemy = ownerObject;
                }
            } 
        }
    }
}