
using System;
using UnityEngine;

[Serializable]
public class PowerupData
{
    public EnumPowerUp Enum;
    public Sprite FaceSprite;
    public Color FaceColor;
    /// <summary>
    /// (0,0) as point of origin
    /// </summary>
    public Vector2Int[] Target;
  
}