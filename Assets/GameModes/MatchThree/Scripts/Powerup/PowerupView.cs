using UnityEngine;
using UnityEngine.UI;

public class PowerupView : MonoBehaviour
{
    private PowerupData PowerupData;
    [SerializeField]
    private Image _graphic; 

    public void SetPowerUpData(PowerupData powerUpData)
    {
        this.PowerupData = powerUpData;
        _graphic.sprite = powerUpData.FaceSprite;
    }

    public PowerupData GetPowerupData() => this.PowerupData;

}
