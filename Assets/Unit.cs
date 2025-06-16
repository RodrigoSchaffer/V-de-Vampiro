using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    public dayTime currentTime;
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
                    if (chosenAttack.hitTargetEffect != null)
                    {
                        StartCoroutine(onHitFX());
                    }
                    break;
                case AttackType.Magical:
                    target.currentHp -= damage;
                    target.tookDmg = true;
                    if (chosenAttack.hitTargetEffect != null)
                    {
                        StartCoroutine(onHitFX());
                    }
                    break;
                case AttackType.Vampiric:
                    if (currentTime == dayTime.Night)
                    {
                        damage += 10;
                        target.currentHp -= damage;
                        target.tookDmg = true;
                        if (chosenAttack.hitTargetEffect != null)
                        {
                            StartCoroutine(onHitFX());
                        }
                        Heal(damage);
                    }
                    else
                    {
                        target.currentHp -= damage;
                        target.tookDmg = true;
                        if (chosenAttack.hitTargetEffect != null)
                        {
                            StartCoroutine(onHitFX());
                        }
                    }                  
                    break;
                case AttackType.Sacred:
                    if (target._unitType == UnitType.Vampire)
                    {
                        damage += 10;
                    }
                    target.currentHp -= damage;
                    target.tookDmg = true;
                    if (chosenAttack.hitTargetEffect != null)
                    {
                        StartCoroutine(onHitFX());
                    }
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
        currentHp += damage;
    }

    public string UseAttack(AttackData attack, AnimationController ac, dayTime time)
    {
        if (currentAp < attack.apCost)
        {
            return $"{unitName} does not have enough AP to use {attack.attackName}.";
        }

        currentAp -= attack.apCost;
        chosenAttack = attack;
        currentTime = time;
        ac.PlayAction(attack._attackAnim);
        return $"{unitName} used {attack.attackName}.";
    }

    public void playProjectileVFX()
    {
            StartCoroutine(moveProjectileVFX());
        
    }

    public IEnumerator moveProjectileVFX()
    {

        if (chosenAttack._attackRange == AttackRange.Ranged)
        {


            GameObject projectile = Instantiate(chosenAttack.projectilePrefab, transform.position, Quaternion.identity);
            float travelSpeed = 10f;


            while (Vector3.Distance(projectile.transform.position, target.transform.position) > 0.1f)
            {
                projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, travelSpeed * Time.deltaTime);
                yield return null;
            }
            Destroy(projectile);
            dealDmg();
        }
        else if (chosenAttack._attackRange == AttackRange.Fall)
        {
            Vector3 attackPos = new Vector3(target.transform.position.x, target.transform.position.y + 1, 0);
            GameObject projectile = Instantiate(chosenAttack.projectilePrefab, attackPos, quaternion.identity);
            yield return new WaitForSeconds(0.4f);
            dealDmg();
            yield return new WaitForSeconds(0.3f);
            Destroy(projectile);
        }
        

    }

    public IEnumerator onHitFX()
    {
        GameObject hitFX = Instantiate(chosenAttack.hitTargetEffect, target.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        if (target.tookDmg == false)
        {

            Destroy(hitFX);

        }
    }

    public void setInactive()
    {
        gameObject.SetActive(false);
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
