using UnityEngine;

public class SlitherZigSegment {
    public Vector3 start;
    public Vector3 end;
    public SlitherZigHitbox hitbox;

    public SlitherZigSegment(Vector3 start,
                             Vector3 end,
                             SlitherZigHitbox hitbox) {
        this.start = start;
        this.end = end;
        this.hitbox = hitbox;
    }

    public void DoDamage(int damageAmount) {
        hitbox.DoDamage(damageAmount);
    }
}