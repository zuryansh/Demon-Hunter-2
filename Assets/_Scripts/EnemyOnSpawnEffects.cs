using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class EnemyOnSpawnEffects : MonoBehaviour
{
    [SerializeField] Transform spriteMask;
    [SerializeField] float animationTime;
    [SerializeField] Collider2D hitCollider;

    // Start is called before the first frame update
    private void Start()
    {
        hitCollider.enabled = false;
    }

    public void StartEffects(float delayTime)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(spriteMask.transform.DOLocalMoveY(0f, animationTime).SetEase(Ease.InOutSine).SetDelay(delayTime));
        sq.OnComplete(()=> hitCollider.enabled = true);
    }
    

}
