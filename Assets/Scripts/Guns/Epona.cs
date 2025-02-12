using UnityEngine;
public class Epona : Gun
{
    public override void TriggerDown()
    {
    }

    public override void TriggerUp()
    {
    }
    public override void Shoot()
    {
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.cyan, 0.01f);
    }
}
