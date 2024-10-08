using UnityEngine;

public enum StunType { Concussion, Freeze }

public class CrowdControllable : ObjectModule {

    [SerializeField] private CCAttributes ccAttributes;
    private RuntimeCCAttributes runtimeProperties;


}