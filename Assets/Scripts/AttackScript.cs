using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script placed on MeleePrefab and RangePrefab PREFABS!!! (which compose the character object children):
public class AttackScript : MonoBehaviour
{
    // what is owner? The gameObject using the prefab. 
    public GameObject owner;

    public FighterStats ownerStats;

    [SerializeField]
    private string animationName;

    [SerializeField]
    private bool magicAttack;

    [SerializeField]
    private float magicCost;

    [SerializeField]
    private float minAttackMultiplier;

    [SerializeField]
    private float maxAttackMultiplier;

    [SerializeField]
    private float minDefenseMultiplier;

    [SerializeField]
    private float maxDefenseMultiplier;

    private FighterStats attackerStats;
    private FighterStats targetStats;
    private float damage = 0.0f;

    // MY CRAP:
    public Animator ownerAnimator;
    public Animator victimAnimator;

    public void Awake() {
        ownerAnimator = owner.GetComponent<Animator>();
    }
    
    // TODO: get global var of victim:
    public void Attack(GameObject victim)
    {
        victimAnimator = victim.GetComponent<Animator>();

        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = victim.GetComponent<FighterStats>();

        if (!targetStats.GetDead())
        {
            if (attackerStats.magic >= magicCost)
            {
                float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);

                damage = multiplier * attackerStats.melee;
                if (magicAttack)
                {
                    damage = multiplier * attackerStats.magicRange;
                }

                float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
                damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

                // animation crap. Where are animations assigned? On the characters themselves (they have an "Animator" component, 
                // which have a "controller", which contains MULTIPLE animations):

                // owner.GetComponent<Animator>().Play(animationName);    
                ownerAnimator.Play(animationName);  

                targetStats.ReceiveDamage(Mathf.CeilToInt(damage));
                attackerStats.updateMagicFill(magicCost);
            } else
            {
                Invoke("SkipTurnContinueGame", 2);
            }  
        } else {
            // ownerAnimator.enabled = false;
            // return;
        }
    }

    void SkipTurnContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController>().NextTurn();
    }
}
