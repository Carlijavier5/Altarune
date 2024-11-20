using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerUtils {

    public const int MAX_RCD = 100;

    public readonly static int GroundLayerMask = 1 << 4;
    public readonly static int EnvironmentLayerMask = 1 << 0;
    public readonly static int SummonHintLayerMask = 1 << 8;
    public readonly static int BaseObjectLayerMask = 1 << 8 & 1 << 9;
}