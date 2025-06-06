using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text nameText;

    public Text hpText;
    public Text apText;
    public Slider hpSlider;

    public void setHuD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpText.text = unit.currentHp.ToString();
        apText.text = unit.currentAp.ToString();
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;
    }

    public void setHp(Unit unit)
    {
        hpSlider.value = unit.currentHp;
        hpText.text = unit.currentHp.ToString();
        apText.text = unit.currentAp.ToString();
    }
    
}
