using UnityEngine;

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

    public bool takeDmg(int dmg)
    {
        if (isBlocking == true)
        {
            currentHp -= dmg - block;
        }
        else
        {
            currentHp -= dmg; 
        }

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

    
}
