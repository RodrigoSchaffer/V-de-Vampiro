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


    public int takeDmg(int dmg)
    {

        if (isBlocking == true)
        {
            int damage = dmg - block;

            return loseHealth(damage);
        }
        else
        {

            return loseHealth(dmg);
        }


    }

    public int loseHealth(int dmg)
    {

        currentHp -= dmg;

        tookDmg = true;
        return dmg;


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
        ac.dmg = attack.damage;
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
