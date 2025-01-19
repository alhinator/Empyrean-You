using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponManager : MonoBehaviour
{
    public Transform LeftArmMount;
    public Transform RightArmMount;

    private ConstraintSource LeftConstraint;
    private ConstraintSource RightConstraint;

    private GameObject LeftWeapon;
    private GameObject RightWeapon;

    [SerializeField] public GameObject[] WeaponPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        //Assign Constraints to their transforms.
        LeftConstraint.sourceTransform = LeftArmMount;
        LeftConstraint.weight = 1f;
        RightConstraint.sourceTransform = RightArmMount;
        RightConstraint.weight = 1f;

        //DEBUG CODE ONLY
        AssignWeapons(1, 1);
    }

    public void AssignWeapons(int left_id, int right_id)
    {
        if (!WeaponPrefabs[left_id] || !WeaponPrefabs[right_id])
        {
            throw new Exception("WeaponManager: AssignWeapon(): Bad left or right id.");
        }
        Destroy(LeftWeapon);
        Destroy(RightWeapon);

        LeftWeapon = Instantiate(WeaponPrefabs[left_id]);
        RightWeapon = Instantiate(WeaponPrefabs[right_id]);


        LeftWeapon.AddComponent<ParentConstraint>().AddSource(LeftConstraint);
        LeftWeapon.GetComponent<ParentConstraint>().constraintActive = true;
        LeftWeapon.transform.localScale = new Vector3(-10, 10, 10);
        RightWeapon.AddComponent<ParentConstraint>().AddSource(RightConstraint);
        RightWeapon.GetComponent<ParentConstraint>().constraintActive = true;


    }


}
