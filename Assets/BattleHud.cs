using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text nameText;

    public Text hpText;
    public Text apText;
    public Slider hpSlider;
    public Slider apSlider;
    public Image unitPic;

    public void setHuD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpText.text = $"{unit.currentHp} / {unit.maxHp}";
        apText.text = unit.currentAp.ToString();
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;
        apSlider.maxValue = unit.maxAp;
        apSlider.value = unit.currentAp;
        if (unit.unitPic != null) {
            
        unitPic.GetComponent<Image>().sprite = unit.unitPic;
        }
    }

    public void setHp(Unit unit)
    {
        hpSlider.value = unit.currentHp;
        hpText.text = unit.currentHp.ToString();
        apSlider.value = unit.currentAp;
        apText.text = unit.currentAp.ToString();
    }
    
}
