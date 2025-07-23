using UnityEngine;

public class GoldenBlock : MonoBehaviour, ISpecialBlockCell
{
    public void OnBlockPlaced()
    {
    }

    public void OnBlockCleared()
    {
    }

    public virtual float GetMultiplyScore()
    {
        return 2.0f;
    }
}
