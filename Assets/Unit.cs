using System.Collections.Generic;
using UnityEngine;


public enum UnitState { ALIVE, DEAD}
public enum UnitTag { PLAYER, ENEMY }
public class Unit : MonoBehaviour
{
    
    public string unitName;
    public int unitDmg;
    public int block;
    public bool isBlocking;
    public int maxHp;
    public int currentHp;
    public int maxAp;
    public int currentAp;
    public bool tookDmg;

    public UnitState state;

    public UnitTag _tag;

    
    public List<AttackData> attacks;

    private AttackData chosenAttack;

    public Unit target;


    public void dealDmg()
    {
        if (target != null) {

            if (isBlocking == true)
            {
                int damage = chosenAttack.damage - target.block;
                if (damage == 0) {
                    damage = 0;                    
                }

                target.currentHp -= damage;
                tookDmg = true;
            }
            else
            {
                target.currentHp -= chosenAttack.damage;
                tookDmg = true;
            }
        }

    }

    public void vampiricHeal(int index)
    {
        currentHp += attacks[index].damage / 2;
    }

    public string UseAttack(AttackData attack, AnimationController ac)
    {
        if (currentAp < attack.apCost)
        {
            return $"{unitName} does not have enough AP to use {attack.attackName}.";
        }

        currentAp -= attack.apCost;
        chosenAttack = attack;
        ac.PlayAction(attack._attackAnim);
        return $"{unitName} used {attack.attackName}.";
    }

    void Update()
    {
        if (currentHp <= 0)
        {
            currentHp = 0;
            state = UnitState.DEAD;
        }
        else
        {
            state = UnitState.ALIVE;
        }

        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }

        if (currentAp > maxAp)
        {
            currentAp = maxAp;
        }
        else if (currentAp < 0)
        {
            currentAp = 0;
        }


    }

}
