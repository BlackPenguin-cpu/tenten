using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private BlockBase _behaviour;

    public BlockBase Behaviour
    {
        get => _behaviour ? _behaviour : null;
        set
        {
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

    public void OnPlace()
    {
        Behaviour?.OnPlaced(this);
    }

    public void OnTurnPass()
    {
        Behaviour?.OnTurnPassed(this);
    }

    public void OnClearBlock()
    {
        Behaviour?.OnClear(this);
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