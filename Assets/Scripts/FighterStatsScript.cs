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
    private GameObject healthFill;

    [SerializeField]
    private GameObject magicFill;

    [Header("Stats")]
    public float health;
    public float magic;

    public float melee;
    public float magicRange;
    public float defense;
    public float speed;
    public float experience;

    private float startHealth;
    private float startMagic;

    [HideInInspector]

    // what's this? some timing BS:
    public int nextActTurn;

    private bool dead = false;

    // Resize health and magic bar
    private Transform healthTransform;
    private Transform magicTransform;

    private Vector2 healthScale;
    private Vector2 magicScale;

    private float xNewHealthScale;
    private float xNewMagicScale;

    private GameObject gameControllerObj;       // need to get "aCharacterIsSelected" property from this

    // this is supposed to be a script:
    private GameController gameControllerScript;

    // MY CRAP:
    [Header("MY CRAP")]
    public Sprite thumbnail;
    public bool actionReady = false;   // depends if its character's turn or not.
    public bool victim = false;        // true when character is victim
    public GameObject ownerObject;
    public float agility;
    public bool isFriendly;
    public Sprite deadSprite;
    public Sprite currentSprite; 
    public bool turnIsOver;
    public Transform circleOutline;
    public bool drawCircle;
    private Transform highlightCursor;
    private bool selected;

    void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startHealth = health;
        startMagic = magic;

        gameControllerObj = GameObject.Find("GameControllerObject");

        if (gameControllerObj != null) {
          gameControllerScript = gameControllerObj.GetComponent<GameController>();
        }

        // MY SHIT:
        // this sprite is CONSTANTLY changing due to animations:
        currentSprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        animator = gameObject.GetComponent<Animator>();

        // I've got MULTIPLE of these, so I shouldn't use tags:
        circleOutline = transform.GetChild(2);

        highlightCursor = transform.GetChild(5);
    }

    void Start() {
        if (currentSprite != null) {
            Debug.Log("current sprite found!");
        }
    }

    // actionReady set in GameController:
    void Update() {
        if (drawCircle) {
            this.circleOutline.gameObject.SetActive(true);
        } 
        else {
            this.circleOutline.gameObject.SetActive(false);
        }
    }

    // this is for the VICTIM:
    public void ReceiveDamage(float damage)
    {
        health = health - damage;
        animator.Play("Damage");

        // Set damage text

        if(health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            Destroy(healthFill);
            // Destroy(gameObject);

            animator.enabled = false;
            // currentSprite = deadSprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        } else if (damage > 0)
        {
            xNewHealthScale = healthScale.x * (health / startHealth);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }
        if (damage > 0)
        {
            // reference to the script on GameController:
            // gameControllerObj.GetComponent<GameController>().battleText.gameObject.SetActive(true);
            // gameControllerObj.GetComponent<GameController>().battleText.text = damage.ToString();
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
    public void CalculateNextTurn(int currentTurn)
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }

    public int CompareTo(object otherStats)
    {
        // WTF IS THIS???
        int nex = nextActTurn.CompareTo(((FighterStatsScript)otherStats).nextActTurn);
        return nex;
    }

    // need reference to the "enemyFace" object, where the display-sprite is:
    // dont think it's best script to put func in?
    // what script to use in?
    public void SetThumbnail()
    {
        Debug.Log(thumbnail.name);
        ownerObject = this.gameObject;

        // the object to put the sprite on, not sprite itself:
        GameObject oppFaceObject = GameObject.Find("EnemyFace");

        // // if player is current victim/attacker:
        if (oppFaceObject != null)
        {
            oppFaceObject.GetComponent<Image>().sprite = thumbnail;
        }
    }

    void OnMouseOver()
    {
        if (gameControllerScript.aCharacterIsSelected == false)
        {
            this.highlightCursor.gameObject.SetActive(true);

            if (!isFriendly) 
            {
                SetThumbnail();
            }
        }
    }

    void OnMouseExit()
    {
        if (!selected)
        {
            this.highlightCursor.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (gameControllerScript.aCharacterIsSelected == false)
        {
            selected = true;
            gameControllerScript.aCharacterIsSelected = true;
        } 
    }

}