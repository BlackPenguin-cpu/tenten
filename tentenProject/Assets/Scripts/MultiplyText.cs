using System.Collections;
using UnityEngine;
using DG.Tweening;

public class MultiplyText : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(MoveText());
    }

    private IEnumerator MoveText()
    {
        transform.DOMoveY(1f, 1.5f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}