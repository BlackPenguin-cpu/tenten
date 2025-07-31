using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lineClearCountText;
    public TextMeshProUGUI blockPlaceCountText;
    public TextMeshProUGUI cellPlaceCountText;

    private BigInteger nowScore = 0;

    private const string lineClearCountTextOriginal = "LineClearCount:";
    private const string blockPlaceCountTextOriginal = "BlockPlaceCount:";
    private const string cellPlaceCountTextOriginal = "CellPlaceCount:";

    private Coroutine coroutine;

    private void Awake()
    {
        instance = this;
    }

    public void InfoApply(ScoreInfo scoreInfo)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ScoreTextAnim(scoreInfo.score));

        lineClearCountText.text = $"{lineClearCountTextOriginal} {scoreInfo.lineClearCount}";
        blockPlaceCountText.text = $"{blockPlaceCountTextOriginal} {scoreInfo.blockPlaceCount}";
        cellPlaceCountText.text = $"{cellPlaceCountTextOriginal} {scoreInfo.cellPlaceCount}";
    }

    private IEnumerator ScoreTextAnim(BigInteger target, float duration = 0.15f)
    {
        scoreText.text = "";
        var str = target.ToString();

        if (target - nowScore >= 1000)
        {
            scoreText.transform.DOShakeRotation(0.15f, 40, 50, 360).onComplete = () =>
                scoreText.transform.Rotate(Vector3.zero);
        }

        nowScore = target;
        var waitSec = new WaitForSeconds(duration / str.Length);
        for (int i = 0; i < str.Length; i++)
        {
            scoreText.text += str[i];
            yield return waitSec;
        }
    }
    // private IEnumerator ScoreCount(int target, float duration = 0.5f)
    // {
    //     if (target - nowScore >= 10000)
    //         scoreText.transform.DOShakePosition(duration, 20, 50,360);
    //
    //     float current = nowScore;
    //     float offset = (target - current) / duration;
    //
    //     while (current < target)
    //     {
    //         current += offset * Time.deltaTime;
    //         scoreText.text = ((int)current).ToString();
    //         yield return null;
    //     }
    //
    //     nowScore = target;
    //     scoreText.text = ((int)current).ToString();
    // }
}