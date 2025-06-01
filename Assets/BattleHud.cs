using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text nameText;
    public Slider hpSlider;

    public void setHuD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;
    }
}
