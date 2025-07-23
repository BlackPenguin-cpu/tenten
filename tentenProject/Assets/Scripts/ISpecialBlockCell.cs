using UnityEngine;

public interface ISpecialBlockCell
{
    public void OnBlockPlaced();
    public void OnBlockCleared();
    public float GetMultiplyScore();
}