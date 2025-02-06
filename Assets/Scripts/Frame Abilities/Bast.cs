using System;
using UnityEngine;

public class Bast : Ability
{
    [Header("Ability-specific variables")]
    private int CurrentKills;
    public int RotationSpeed = 30;
    public bool isShielded;
    public GameObject shieldModel;

    void Start()
    {
        shieldModel = GameObject.FindWithTag("BastShield");
        if (shieldModel == null) { throw new System.Exception("Bast: Constructor: Could not find shield model"); }
        shieldModel.SetActive(false);
    }
    void Update()
    {
        if (isShielded)
        {
            float x = Mathf.Sin(Time.time) * RotationSpeed * Time.deltaTime;
            float y = Mathf.Sin(Time.time) * RotationSpeed * Time.deltaTime;
            float z = Mathf.Cos(Time.time) * RotationSpeed * Time.deltaTime;
            Vector3 dir = new Vector3(x, y, z);
            shieldModel.transform.Rotate(dir);
            shieldModel.transform.GetChild(0).transform.Rotate(dir * -1);
        }
    }
    public override void OnKill(DamageInstance d)
    {
        base.OnKill(d);
        CurrentKills++;
        if (CurrentKills >= Owner.CurrentHP)
        {
            isShielded = true;
            shieldModel.SetActive(true);
            //Do shield animation.
        }
    }

    public override bool HitDetected(DamageInstance d)
    {
        if (isShielded)
        {
            isShielded = false;
            shieldModel.SetActive(false);
            CurrentKills = 0;
            //Do break shield animation
            return false;
        }
        else
        {
            return true;
        }
    }

}