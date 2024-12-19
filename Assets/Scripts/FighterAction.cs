using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// one of two scripts placed on the character objects:
public class FighterAction : MonoBehaviour
{
    private GameObject hero;
    private GameObject enemy;

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject rangePrefab;

    [SerializeField]
    private Sprite faceIcon;

    private GameObject currentAttack;

    // MY CRAP:
    private GameObject friendliesParent;
    private GameObject enemiesParent;
    
    void Awake()
    {
        hero = GameObject.FindGameObjectWithTag("Hero");
        enemy = GameObject.FindGameObjectWithTag("Enemy");

        friendliesParent = GameObject.Find("Friendlies");           // success
        enemiesParent = GameObject.Find("Enemies");
    }

    void Start() {
        if (friendliesParent != null && enemiesParent != null)
        {
            Debug.Log("character parent objects found");
        }
    }

    // what's this?
    public void SelectAttack(string btn)
    {
        GameObject victim = hero;
        if (tag == "Hero")                      // what is this tag?
        {
            victim = enemy;
        }
        if (btn.CompareTo("melee") == 0)
        {
            meleePrefab.GetComponent<AttackScript>().Attack(victim);

        } else if (btn.CompareTo("range") == 0)
        {
            rangePrefab.GetComponent<AttackScript>().Attack(victim);
        } else
        {
            Debug.Log("Run");
        }
    }
}
