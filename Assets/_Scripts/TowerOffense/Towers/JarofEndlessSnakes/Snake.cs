using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
public partial class Snake : Entity
{
    [SerializeField] private float visionRadius;
    [SerializeField] private NavMeshAgent navMeshAgent;
    private Entity target;
    private StateMachine<Snake_Input> stateMachine = new();
    public int id = 0;
    public JarOfEndlessSnakes parent;
    void Awake()
    {
        Snake_Input input = new Snake_Input(stateMachine, this);
        stateMachine.Init(input, new State_Idle());
    }

    protected override void Update()
    {
        base.Update();
        if(target == null) target = null;
        stateMachine.Update();
    }

    public bool getTarget(){
        if (target != null && (navMeshAgent.transform.position - target.transform.position).magnitude < visionRadius){
            return true;
        }
        Collider[] hitColliders = Physics.OverlapSphere(navMeshAgent.transform.position, visionRadius);
        float closestDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out Entity entity)){
                if(hitCollider.TryGetComponent(out Transform transform)){
                    float distance = (navMeshAgent.transform.position - parent.transform.position).magnitude;
                    if (distance < closestDistance && entity != null && entity.Faction == EntityFaction.Hostile){
                        closestDistance = distance;
                        target = entity;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override void Perish(bool immediate)
    {
        base.Perish(immediate);
        parent.deleteSnake(id);
    }
}
