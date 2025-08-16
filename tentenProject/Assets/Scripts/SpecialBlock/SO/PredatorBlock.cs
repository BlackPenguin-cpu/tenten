using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/PredatorBlock")]
public class PredatorBlock : BlockBase
{
    public TextMeshPro multiplyText;
    public TextMeshPro predatorText;

    public override IBlockRuntime CreateRuntime()
    {
        return new RuntimePredatorBlock(this);
    }
}