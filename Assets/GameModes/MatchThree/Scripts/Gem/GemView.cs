using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GemView : MonoBehaviour
{
    private GemData GemData;
    [SerializeField]
    private Image _graphic;
    public Action<GemView> Release;

    public void SetGemData(GemData gemData)
    {
        this.GemData = gemData;
        _graphic.sprite = GemData.FaceSprite;
    }

    public GemData GetGemData() => this.GemData;

    public Image GetGraphic() => this._graphic;

    public void SwapAnimation(GemView targetGemView)
    {
        var speed = 50f;

        var targetGemViewPos = targetGemView.transform.position;
        var targetOwnPos = transform.position;

        var parent = transform.parent;

        transform.SetParent(targetGemView.transform.parent);
        targetGemView.transform.SetParent(parent);

        transform.DOMove(targetGemViewPos, speed).SetSpeedBased(true).SetEase(Ease.OutCirc);
        targetGemView.transform.DOMove(targetOwnPos, speed).SetSpeedBased(true).SetEase(Ease.OutCirc);
    }
}
