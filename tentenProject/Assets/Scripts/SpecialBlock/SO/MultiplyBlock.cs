using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Block/Behavior/MultiplyBlock")]
public class MultiplyBlock : BlockBase
{
    [FormerlySerializedAs("multipliey")] [FormerlySerializedAs("multiplier")] public float multiply = 2.0f;
    public override IBlockRuntime CreateRuntime()
    {
        return new RuntimeMultiplyBlock(this);
    }
}
