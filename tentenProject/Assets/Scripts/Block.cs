using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool alreadyUsed = false;
    [SerializeField] private BlockBase _behaviour;

    public BlockBase Behaviour
    {
        get => _behaviour ? _behaviour : null;
        set
        {
            if(_behaviour != value)
                alreadyUsed = false;
            _behaviour = value;
            InfoApply();
        }
    }

    [ContextMenu("iconApply")]
    private void InfoApply()
    {
        if (_behaviour != null)
            GetComponent<SpriteRenderer>().sprite = Behaviour.icon;
    }

    public void OnPlace(List<Block> cellList, Vector2Int cellPos)
    {
        Behaviour?.OnPlaced(cellList, cellPos);
    }

    public void OnTurnPass(List<Block> cellList, Vector2Int cellPos)
    {
        Behaviour?.OnTurnPassed(cellList, cellPos);
    }

    public async UniTask OnClearBlock(List<Block> cellList, Vector2Int cellPos)
    {
        if(alreadyUsed) return;
        alreadyUsed = true;
        
        if (Behaviour != null)
            await Behaviour.OnClear(cellList, cellPos);
        
        Behaviour = null;
        alreadyUsed = false;
    }

    public float GetScoreMultiply()
    {
        return Behaviour != null
            ? Behaviour.GetScoreMultiply(this)
            : 1;
    }

    public float GetBaseScore()
    {
        return Behaviour != null
            ? Behaviour.baseScore
            : 50;
    }
}