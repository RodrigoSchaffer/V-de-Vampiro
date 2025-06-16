using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackInfo : MonoBehaviour
{
    public GameObject infoPanel;
    [SerializeField] public dayTime dayOrNight;
    public TextMeshProUGUI damageText, apCost, descriptionText, wins;
    public AttackData attacks;

    public void ShowAttackInfo(AttackData attack)
    {
        switch (dayOrNight)
        {
            case dayTime.Day:
                infoPanel.SetActive(true);
                damageText.text = "Damage: " + attack.damage;
                apCost.text = "Ap Cost: " + attack.apCost;
                descriptionText.text = attack.description;
                break;
            case dayTime.Night:
                int moonLitAttack = attack.damage + 10;
                infoPanel.SetActive(true);
                damageText.text = "Damage: " + moonLitAttack;
                apCost.text = "Ap Cost: " + attack.apCost;
                descriptionText.text = attack.description + " And heals itself by the damage amount";
                break;
        }

    }

    public void showBlockInfo(Unit unit)
    {
        switch (dayOrNight)
        {
            case dayTime.Day:
                infoPanel.SetActive(true);
                damageText.text = "Block: " + unit.block;
                apCost.text = "Ap Cost: " + 1;
                descriptionText.text = "Reduce incoming damage by " + unit.block;
                break;
            case dayTime.Night:
                int moonLitBlock = unit.block + 15;
                infoPanel.SetActive(true);
                damageText.text = "Block: " + moonLitBlock;
                apCost.text = "Ap Cost: " + 1;
                descriptionText.text = "Reduce incoming damage by " + moonLitBlock;
                break;
        }
    }

    public void HideAttackInfo()
    {
        infoPanel.SetActive(false);
    }
    public void winCount(int wins)
    {
        this.wins.text = "WINS: " + wins;
    }

    public void setTime(dayTime time)
    {
        dayOrNight = time;
    }

}
