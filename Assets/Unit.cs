using UnityEditor.UI;
using UnityEngine;


public enum UnitState { ALIVE, DEAD, POISONED, MOONLIT, SUNBURNT }
public enum UnitTag{ PLAYER, ENEMY, BOSS}
public class Unit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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


    public int takeDmg(int dmg)
    {

        if (isBlocking == true)
        {
            int damage = dmg - block;
            tookDmg = true;
            return loseHealth(damage);
        }
        else
        {
            tookDmg = true;
            return loseHealth(dmg);
        }


    }

    public int loseHealth(int dmg)
    {

        currentHp -= dmg;

        tookDmg = false;
        return dmg;


    }

    public bool isDead()
    {
        if (currentHp <= 0)
        {
            currentHp = 0;
            return true;
        }
        else
        {
            return false;
        }
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
    }




}
