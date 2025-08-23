using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2Int pos;
    public bool alreadyUsed = false;
    [SerializeField] private BlockBase _behaviour;
    public IBlockRuntime _runtimeBlock;

    public BlockBase Behaviour
    {
        get => _behaviour ? _behaviour : null;
        set
        {
            if (_behaviour != value)
                alreadyUsed = false;
            _behaviour = value;
            
            _runtimeBlock = _behaviour != null ? value.CreateRuntime() : null;
            InfoApply();
        }
    }

    [ContextMenu("iconApply")]
    private void InfoApply()
    {
        if (_behaviour != null)
            GetComponent<SpriteRenderer>().sprite = Behaviour.icon;
    }

    public void OnPlace()
    {
        _runtimeBlock?.OnPlaced(pos);
    }

    public void OnTurnPass()
    {
        _runtimeBlock?.OnTurnPassed(pos);
    }

    public async UniTask OnClearBlock()
    {
        if (alreadyUsed) return;
        alreadyUsed = true;

        if (Behaviour != null)
            await _runtimeBlock.OnClear(pos);

        Behaviour = null;
        _runtimeBlock = null;
        alreadyUsed = false;
    }

    public float GetScoreMultiply()
    {
        return Behaviour != null
            ? _runtimeBlock.GetScoreMultiply()
            : 1;
    }

    public float GetBaseScore()
    {
        return Behaviour != null
            ? Behaviour.baseScore
            : 50;
    }
}