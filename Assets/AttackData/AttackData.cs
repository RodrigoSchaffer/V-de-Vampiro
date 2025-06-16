using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public int damage;
    public int apCost;
    public string description;
    public AttackType type;
    public AttackRange _attackRange;
    public string _attackAnim;
    public GameObject projectilePrefab;
    public GameObject hitTargetEffect;

    
}

public enum AttackType
{
    Physical,
    Magical,
    Vampiric,
    Sacred,
    Status
}
public enum AttackRange
{
    Melee,
    Ranged,
    Fall
}