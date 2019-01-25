using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public Vector2[] blockSizes =
        new Vector2[] {
        new Vector2(6,5),
        new Vector2(4,3)
        };

    public string[] block =
        new string[]{
        @"
        abbaaa
        aaaaaa
        abbbab
        aaabaa
        aabbab
        ",
        @"
        aabb
        aaab
        abaa
        "
        };
}
