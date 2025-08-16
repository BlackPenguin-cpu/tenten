using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/SquareBomb")]
public class SquareBomb : BlockBase
{
    public override IBlockRuntime CreateRuntime()
    {
        return new RuntimeSquareBomb(this);
    }
}