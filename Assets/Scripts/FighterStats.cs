using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// when does this run? at beginning. Runs multiple times simult. if there's mult. characters

// placed where? Individual characters
// placed on INDIVIDUAL instances!
public class FighterStats : MonoBehaviour, IComparable
{
    [SerializeField]
    private Animator animator;          // how's this assigned?

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

    private GameObject GameControllerObj;

    // MY CRAP:
    public Sprite thumbnail;
    public GameObject ownerObject;
    public bool actionReady;            // depends if its character's turn or not.

    [Header("MY SHIT")]
    public float agility;
    public bool isFriendly;
    public Sprite deadSprite;
    public Sprite currentSprite; 
    public bool turnIsOver;

    void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startHealth = health;
        startMagic = magic;

        GameControllerObj = GameObject.Find("GameControllerObject");

        // MY SHIT:
        // this sprite is CONSTANTLY changing due to animations:
        currentSprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        animator = gameObject.GetComponent<Animator>();
    }

    void Start() {
        Debug.Log("FUCK!");

        if (currentSprite != null) {
            Debug.Log("current sprite found!");
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
        if(damage > 0)
        {
            GameControllerObj.GetComponent<GameController>().battleText.gameObject.SetActive(true);
            GameControllerObj.GetComponent<GameController>().battleText.text = damage.ToString();
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
        int nex = nextActTurn.CompareTo(((FighterStats)otherStats).nextActTurn);
        return nex;
    }

    // need reference to the "enemyFace" object, where the display-sprite is:
    public void SetThumbnail()
    {
        ownerObject = this.gameObject;

        //var GameObject faceObject = GameObject.Find("EnemyFace");

        // if (faceObject != null && actionReady == true)
        // {
        //     faceObject.GetComponent<Image>().sprite = thumbnail;
        // }
    }

}