using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public int damage;
    public int apCost;
    public string description;
    public AttackType type;
    public string _attackAnim;

    
}

public enum AttackType
{
    Physical,
    Magical,
    Vampiric,
    Status
}