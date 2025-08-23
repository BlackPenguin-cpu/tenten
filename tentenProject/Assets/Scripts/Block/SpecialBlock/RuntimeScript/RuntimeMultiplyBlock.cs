using Cysharp.Threading.Tasks;
using UnityEngine;

public class RuntimeMultiplyBlock : IBlockRuntime
{
    private readonly MultiplyBlock block;

    public RuntimeMultiplyBlock(MultiplyBlock data)
    {
        block = data;
    }

    public void OnPlaced(Vector2Int pos)
    {
    }

    public void OnTurnPassed(Vector2Int pos)
    {
    }

    public async UniTask OnClear(Vector2Int pos)
    {
        await UniTask.Delay(10);
    }


    public float GetScoreMultiply()
    {
        return block.multiply;
    }
}