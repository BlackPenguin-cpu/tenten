using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lineClearCountText;
    public TextMeshProUGUI blockPlaceCountText;
    public TextMeshProUGUI cellPlaceCountText;

    private const string scoreTextOriginal = "Score:";
    private const string lineClearCountTextOriginal = "LineClearCount:";
    private const string blockPlaceCountTextOriginal = "BlockPlaceCount:";
    private const string cellPlaceCountTextOriginal = "CellPlaceCount:";

    private void Awake()
    {
        instance = this;
    }

    public void InfoApply(ScoreInfo scoreInfo)
    {
        scoreText.text = $"{scoreTextOriginal} {scoreInfo.score}";
        lineClearCountText.text = $"{lineClearCountTextOriginal} {scoreInfo.lineClearCount}";
        blockPlaceCountText.text = $"{blockPlaceCountTextOriginal} {scoreInfo.blockPlaceCount}";
        cellPlaceCountText.text = $"{cellPlaceCountTextOriginal} {scoreInfo.cellPlaceCount}";
    }

    private IEnumerator ScoreCount(float target, float duration = 0.5f)
    {
        float current = int.Parse(scoreText.text);
        float offset = (target - current) / duration;

        while (current < target)
        {
            current += offset * Time.deltaTime;
            scoreText.text = ((int)current).ToString();
            yield return null;
        }

        current = target;
        scoreText.text = ((int)current).ToString();
    }
}