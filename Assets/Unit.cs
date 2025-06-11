using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public enum UnitState { ALIVE, DEAD}
public enum UnitTag { PLAYER, ENEMY }

public enum UnitType {Vampire, Sacred, Giant, Monster}
public class Unit : MonoBehaviour
{

    public string unitName;
    public int block;
    public bool isBlocking;
    public int maxHp;
    public int currentHp;
    public int maxAp;
    public int currentAp;
    public bool tookDmg;
    [SerializeField]public Sprite unitPic;

    public UnitState state;

    public UnitTag _tag;

    public UnitType _unitType;


    public List<AttackData> attacks;

    private AttackData chosenAttack;

    public Unit target;


    public void dealDmg()
    {
        int damage = 0;
        if (target != null)
        {

            if (isBlocking == true)
            {
                damage = chosenAttack.damage - target.block;
                if (damage == 0)
                {
                    damage = 0;
                }

                
            }
            else
            {
                damage = chosenAttack.damage;
                if (damage == 0)
                {
                    damage = 0;
                }
                
            }


            switch (chosenAttack.type)
            {
                case AttackType.Physical:
                    target.currentHp -= damage;
                    target.tookDmg = true;
                    break;
                case AttackType.Magical:
                    target.currentHp -= damage;
                    target.tookDmg = true;
                    break;
                case AttackType.Vampiric:
                    target.currentHp -= damage;
                    target.tookDmg = true;
                    Heal(damage);
                    break;
            }
        }
        else
        {
            Debug.Log("No Target Assigned");
        }

    }

    public void Heal(int damage)
    {
        currentHp += damage / 2;
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
