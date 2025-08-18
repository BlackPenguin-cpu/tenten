using System.Numerics;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lineClearCountText;
    public TextMeshProUGUI blockPlaceCountText;
    public TextMeshProUGUI cellPlaceCountText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI remainBlockText;

    private BigInteger nowScore = 0;

    private const string targetScoreTextOriginal = "Target Score:";
    private const string remainBlockTextOriginal = "Remain Block Count:";

    private const string lineClearCountTextOriginal = "LineClearCount: ";
    private const string blockPlaceCountTextOriginal = "BlockPlaceCount: ";
    private const string cellPlaceCountTextOriginal = "CellPlaceCount: ";

    private Coroutine coroutine;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private int prevPlaceBlockNum = 0;

    private StageDataClass stageData;

    private void Awake()
    {
        instance = this;
    }

    public void InfoApply(ScoreInfo scoreInfo)
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = new CancellationTokenSource();

        ScoreTextAnim(scoreInfo.score);

        lineClearCountText.text = $"{lineClearCountTextOriginal} {scoreInfo.lineClearCount}";
        blockPlaceCountText.text = $"{blockPlaceCountTextOriginal} {scoreInfo.blockPlaceCount}";
        prevPlaceBlockNum = scoreInfo.blockPlaceCount;
        cellPlaceCountText.text = $"{cellPlaceCountTextOriginal} {scoreInfo.cellPlaceCount}";
        
        remainBlockText.text = $"{remainBlockTextOriginal}{stageData.canPlaceBlock - prevPlaceBlockNum}";
    }

    public void StageUIApply(StageDataClass stageDataClass)
    {
        stageData = stageDataClass;
        stageText.text = $"{stageData.stageNum}-{stageData.chapterNum}";
        targetScoreText.text = $"{targetScoreTextOriginal}{stageData.targetScore}";
        remainBlockText.text = $"{remainBlockTextOriginal}{stageData.canPlaceBlock - prevPlaceBlockNum}";
    }

    private async UniTask ScoreTextAnim(BigInteger target, float duration = 0.3f)
    {
        scoreText.text = "";
        var str = target.ToString();

        if (target - nowScore >= 100)
        {
            scoreText.transform.DOShakeRotation(duration, 100, 90, 360)
                .onComplete += () => { scoreText.transform.rotation = quaternion.identity; };
        }

        nowScore = target;
        await TextAnim(scoreText, str, duration, _cts.Token);
    }

    public static async UniTask TextAnim(TMP_Text targetObj, string text, float duration = 0.3f,
        CancellationToken token = default)
    {
        var ct = CancellationToken.None;
        if (token == default)
            ct = targetObj.GetCancellationTokenOnDestroy();
        else
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                token, targetObj.GetCancellationTokenOnDestroy());
            ct = linkedCts.Token;
        }

        targetObj.text = "";
        foreach (var t in text)
        {
            ct.ThrowIfCancellationRequested();
            targetObj.text += t;
            await UniTask.WaitForSeconds(duration / text.Length, cancellationToken: ct);
        }
    }
}