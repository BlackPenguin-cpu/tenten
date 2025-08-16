using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/CrossBomb")]
public class CrossBomb : BlockBase
{
    public override IBlockRuntime CreateRuntime()
    {
        return new RuntimeCrossBomb(this);
    }
}