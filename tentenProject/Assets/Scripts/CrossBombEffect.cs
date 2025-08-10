using DG.Tweening;
using UnityEngine;

public class CrossBombEffect : MonoBehaviour
{
    void Start()
    {
        StartEffect();
    }

    private void StartEffect()
    {
        GetComponent<SpriteRenderer>().DOFade(0,0.2f);
        transform.DOScale(0, 0.2f).onComplete = () => { Destroy(gameObject); };
    }
}