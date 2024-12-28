using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// when does this run? at beginning. Runs multiple times simult. if there's mult. characters

// placed where? Individual characters
// placed on INDIVIDUAL instances!
public class FighterStatsScript : MonoBehaviour, IComparable
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
    public float speed;
    public float experience;

    private float startHealth;          // and this? the initial value of "health"
    private float startMagic;

    [HideInInspector]

    // what's this? some timing BS:
    public int nextActTurn;

    public bool dead = false;

    // Resize health and magic bar
    public Transform healthTransform;          // this is the Transform component of the HealthFill object.
    private Transform magicTransform;

    public Vector2 healthScale;                // what's this? a 2-value tuple.    used to change the size of healthbar
    private Vector2 magicScale;

    public float xNewHealthScale;
    private float xNewMagicScale;

    private GameObject gameControllerObj;       // need to get "aCharacterIsSelected" property from this

    // this is supposed to be a script:
    private GameController gameControllerScript;

    // MY CRAP:
    [Header("MY CRAP")]
    public Sprite thumbnail;           // each character has their own thumbnail assigned manually
    public bool actionReady = false;   // depends if its character's turn or not.
    public bool victim = false;        // true when character is victim
    public float agility;               
    public bool isFriendly;
    public Sprite deadSprite;
    public Sprite currentSprite; 
    public bool turnIsOver;
    public Transform circleOutlineGreen;
    public Transform circleOutlineYellow;
    public Transform circleOutlineRed;
    public Transform currentCircleOutline;

    public bool drawCircle;
    public Transform highlightCursor;
    public bool selected;
    public GameObject ownerObject;
    public FighterAction playerActionScript;
    public float damageTaken = 0f;
    public bool hoveringOver = false;      // use to override aCharacterIsSelected

    // PRIORITY LIST CRAP:
    public GameObject currentPriorityCharacter;
    public bool isCurrentPriorityCharacter;

    void Awake()
    {
        ownerObject = this.gameObject;

        // healthFill = transform.GetChild(6).gameObject;

        playerActionScript = GameObject.Find("WizardHero").GetComponent<FighterAction>();

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
        if (currentSprite != null) {
            Debug.Log("current sprite found!");
        }
    }

    // actionReady set in GameController:
    void Update() {
        DrawCircle();
    }

    void DrawCircle() {

        // this shit is fucked:
        if (health >= 80) {
            currentCircleOutline = circleOutlineGreen;
        } 
        else if (health >= 45 && health <= 79) {
            currentCircleOutline = circleOutlineYellow;
        }
        else {
            currentCircleOutline = circleOutlineRed;
        }

        if (drawCircle) {
            this.currentCircleOutline.gameObject.SetActive(true);
        } 
        else {
            this.currentCircleOutline.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && selected == true) {
            selected = false;
            this.highlightCursor.gameObject.SetActive(false);
            // activateCursor();
            gameControllerScript.aCharacterIsSelected = false;
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
        Debug.Log(health);

        animator.Play("Damage");

        if(health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            gameControllerScript.selectedCharacter = null; // successfully makes it empty

            animator.enabled = false;

            selected = false;

            gameControllerScript.aCharacterIsSelected = false;

            gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;

            // this actually do anything?
            highlightCursor.gameObject.SetActive(false);

            // activateCursor();

            // REMOVE DEAD CHARACTER FROM CHARACTERLIST AND PRIORITYLIST:
            Debug.Log(gameControllerScript.charactersList.Count);
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

    void ContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }

    public int CompareTo(object otherStats)
    {
        // WTF IS THIS???
        int nex = nextActTurn.CompareTo(((FighterStatsScript)otherStats).nextActTurn);
        return nex;
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
        Debug.Log(health);
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

    public void CursorHandler()
    {
        return;
    }

    void OnMouseOver()
    {
        // why isn't this being set to true???
        hoveringOver = true; 
        Debug.Log($"HOVERING OVER ENEMY: {this.name}");

        // new macro if statement:
        if (!gameControllerScript.cursorAlreadyActive) {
            if (hoveringOver == true && !dead) {
                this.highlightCursor.gameObject.SetActive(true);
                gameControllerScript.cursorAlreadyActive = true;
            }

            // if another character isn't already selected:
            if (gameControllerScript.aCharacterIsSelected == false && !dead)
            {
                selected = false;
                this.highlightCursor.gameObject.SetActive(true);
                gameControllerScript.cursorAlreadyActive = true;

                if (!isFriendly) 
                {
                    SetEnemyThumbnail();
                    SetEnemyHealth();
                }
            }
        }
    }

    void OnMouseExit()
    {
        hoveringOver = false;
        gameControllerScript.aCharacterIsSelected = false;
        if (!selected)
        {
            this.highlightCursor.gameObject.SetActive(false);

            // DOES NOT FUCKING WORK:  
            if (!gameControllerScript.aCharacterIsSelected) {
                gameControllerScript.cursorAlreadyActive = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (!gameControllerScript.aCharacterIsSelected) {
            SelectNewCharacter();
        }
    }

    // should run both automatically and OnMouseDown:
    public void SelectNewCharacter() 
    {
        // why not just run this shit when new character's selected automatically?
        if ((gameControllerScript.aCharacterIsSelected == false && !dead) || (hoveringOver == true && !dead))
        {
            selected = true;
            gameControllerScript.aCharacterIsSelected = true;
            gameControllerScript.selectedCharacter = ownerObject;
            gameControllerScript.actionMenu.SetActive(true);

            // makes it so that next enemy to attack is the one selected:
            playerActionScript.enemy = ownerObject;

            // healthFill = transform.GetChild(6).gameObject;

            Debug.Log($"new character selected: {gameControllerScript.selectedCharacter}");
        } 
    }
}