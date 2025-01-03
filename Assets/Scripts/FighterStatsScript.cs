using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// when does this run? at beginning. Runs multiple times simult. if there's mult. characters

// placed where? Individual characters
// placed on INDIVIDUAL instances!

public class FighterStatsScript : MonoBehaviour
{
    [SerializeField]
    private Animator animator;          // how's this assigned? manually

    [SerializeField]

    // change this when new character selected:
    private GameObject healthFill;      // and this? assigned manually, initially. found in the character's children.

    [SerializeField]
    private GameObject magicFill;

    [Header("Stats")]
    public float health;                // what's this? a manually entered NUMBER
    public float magic;

    public float melee;
    public float magicRange;
    public float defense;

    private float startHealth;          // and this? the initial value of "health"
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
    public FighterAction playerActionScript;
    public float damageTaken = 0f;

    // is this var only ever used internally? YES
    public bool selected;
    public bool hoveringOver = false;      
    public bool drawTheCircle;              // should be true when current character's turn
    public bool turnIsOver;

    // move towards enemy when this is true. As soon as false, BAD.
    public bool turnInProgress;            // could use this to determine when to draw circle   --SUCCESS

    public bool isSniper;           // so that exclusively range enemies dont move towards opponent

    public bool attacking = false;      // turns false as soon as character starts returning. Turns true when? as soon as we move towards enemy
    public bool retreating = false;

    // position where player stands before moving to enemy. Should be IMMUTABLE:
    public Vector3 originalPosition;        // SUCCESS  

    void Awake()
    {
        ownerObject = this.gameObject;

        if (ownerObject != null) {
            originalPosition = ownerObject.transform.position;
        }

        SetHealth();

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startMagic = magic;

        gameControllerObj = GameObject.Find("GameControllerObject");

        if (gameControllerObj != null) {
          gameControllerScript = gameControllerObj.GetComponent<GameController>();
        }

        // MY SHIT:
        // this sprite is CONSTANTLY changing due to animations:
        currentSprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        animator = gameObject.GetComponent<Animator>();

        circleOutlineGreen = transform.GetChild(2);
        circleOutlineYellow = transform.GetChild(3);
        circleOutlineRed = transform.GetChild(4);

        highlightCursor = transform.GetChild(5);
    }

    void Start() {
        playerActionScript = gameControllerScript.currentHeroObj.GetComponent<FighterAction>();
    }

    void Update() {
        // moving this from DrawCircle() to here fixed Escape issue:
        if (Input.GetKeyDown(KeyCode.Escape) && selected == true && gameControllerScript.freeState == true) {
            selected = false;
            this.highlightCursor.gameObject.SetActive(false);

            // NEW ADDITION:
            gameControllerScript.cursorAlreadyActive = false; 
            gameControllerScript.characterManuallySelected = false;
        }

        if (turnInProgress == true) {
            DrawCircle();
        } else if (currentCircleOutline != null) {
            this.currentCircleOutline.gameObject.SetActive(false);
        }
        // DrawCircle();
    }

    void DrawCircle() {
        // this shit is fucked:
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

    // used only once in awake():
    void SetHealth()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

         startHealth = health;
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

            // NEW ADDITION: 
            gameControllerScript.cursorAlreadyActive = false; 

            // REMOVE DEAD CHARACTER FROM CHARACTERLIST AND PRIORITYLIST:
            Debug.Log($"REMOVING CHARACTER FROM LIST: {this.name}");
            gameControllerScript.charactersList.Remove(gameObject);
            Debug.Log(gameControllerScript.charactersList.Count);

            gameControllerScript.priorityList.Remove(gameObject);
            gameControllerScript.AutoSelectNextEnemy();

        } else if (damage > 0)
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

    // WTF IS THIS????? sus
    void ContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }

    // this and SetEnemyHealth() seem like they'd be better off in GameController? Nah, we're good. !isFriendly comes into play
    public void SetEnemyThumbnail()
    {
        GameObject oppFaceObject = GameObject.Find("EnemyFace");

        if (oppFaceObject != null)
        {
            oppFaceObject.GetComponent<Image>().sprite = thumbnail;
        }
    }

    // used when HOVERING over enemy:
    // what's healthFill again? gameObject containing the image.
    // REMEMBER: the size of the health is set in ReceiveDamage() in FighterStatsScript.
    public void SetEnemyHealth()
    {
        // everything in here is LOCAL:

        // can confirm these are unique:
        GameObject oppHealthBar = transform.GetChild(6).gameObject;

        Sprite oppHealthBarImage = oppHealthBar.GetComponent<Image>().sprite;       

        GameObject oppMenuHealthDisplay = GameObject.Find("EnemyHealthFill");

        if (oppHealthBarImage != null && oppMenuHealthDisplay != null) 
        {
            // CHANGES THE IMAGE, BUT NOT THE QUANTITY:
            oppMenuHealthDisplay.GetComponent<Image>().sprite = oppHealthBarImage;

            // NEW: 
            health = health - damageTaken;
            float myNewHealthScale = healthScale.x * (health / startHealth);

            // x size changes based on the health:
            oppHealthBar.transform.localScale = new Vector2(myNewHealthScale, healthScale.y);
        }
    }

    void OnMouseOver()
    {
        hoveringOver = true; 

        Debug.Log($"{this.name} is at coords: {this.originalPosition}");

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

                    if (!isFriendly) 
                    {
                        SetEnemyThumbnail();
                        SetEnemyHealth();
                        SetEnemyName();
                    }
                }
            }
        }
    }

    void SetEnemyName() {
       GameObject oppNameObject = GameObject.Find("EnemyFrameLabel"); 
       oppNameObject.GetComponent<Text>().text = this.name;
    }

    // with 4 characters, all of this is FUCKED:
    // THE BUGGER IS HERE:
    void OnMouseExit()
    {
        hoveringOver = false;

        // if (selected) {
        //     gameControllerScript.cursorAlreadyActive = false; 
        // }

        // this only works when no character's manually selected:
        // if the current character is NOT selected (all of them initially):
        if (!selected && !gameControllerScript.characterManuallySelected)
        {
            this.highlightCursor.gameObject.SetActive(false);

            // NEW ADDITION: 
            gameControllerScript.cursorAlreadyActive = false;   
        }
    }

    void OnMouseDown()
    {
        SelectNewCharacter();
    }

    // should run both automatically and OnMouseDown:
    public void SelectNewCharacter() 
    {
        if (gameControllerScript.freeState == true) {
            gameControllerScript.characterManuallySelected = true;
            if ((gameControllerScript.selectedCharacter != null && !dead) || (hoveringOver == true && !dead))
            {
                selected = true;
                gameControllerScript.selectedCharacter = ownerObject;
                gameControllerScript.actionMenu.SetActive(true);

                // makes it so that next enemy to attack is the one selected:
                playerActionScript.enemy = ownerObject;
            } 
        }
    }
}