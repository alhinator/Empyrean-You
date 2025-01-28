using UnityEngine;
public class Artemis : Gun
{
    public override void TriggerDown()
    {
        Debug.Log("In triggerdowm artemis");
        Shoot();
    }

    public override void TriggerUp()
    {
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.red, 0.01f);

    }
    protected override void Shoot()
    {
        Debug.DrawRay(bulletOrigin.position, bulletOrigin.forward * 100, Color.cyan, 0.01f);
    }

}
