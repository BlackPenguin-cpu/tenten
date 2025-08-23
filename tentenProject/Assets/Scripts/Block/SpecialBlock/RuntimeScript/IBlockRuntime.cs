using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IBlockRuntime
{
    void OnPlaced(Vector2Int pos);
    void OnTurnPassed(Vector2Int pos);
    UniTask OnClear(Vector2Int pos);
    float GetScoreMultiply();
}

public interface IBlockHaveCleanEffect
{
    void EffectClean();
}