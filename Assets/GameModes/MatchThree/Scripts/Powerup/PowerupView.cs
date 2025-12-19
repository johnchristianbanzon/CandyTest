using UnityEngine;
using UnityEngine.UI;

public class PowerupView : MonoBehaviour
{
    [SerializeField]
    private PowerupData PowerupData;
    [SerializeField]
    private Image _graphic; 

    public void SetPowerUpData(PowerupData powerUpData)
    {
        this.PowerupData = powerUpData;
        _graphic.sprite = powerUpData.FaceSprite;
        _graphic.color = powerUpData.FaceColor;
    }

    public PowerupData GetPowerupData() => this.PowerupData;

}
