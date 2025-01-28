using System;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public Transform LeftArmMount;
    public Transform RightArmMount;

    private ConstraintSource LeftConstraint;
    private ConstraintSource RightConstraint;

    private GameObject LeftWeapon;
    private GameObject RightWeapon;
    private Gun mainGun;
    private Gun offGun;

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

        //Get their gun scripts
        mainGun = RightWeapon.GetComponent<Gun>();
        offGun = LeftWeapon.GetComponent<Gun>();
        if(mainGun == null || offGun == null){
            throw new System.Exception("PlayerController: Start: Missing a gun.");
        } else {
            Debug.Log(offGun.GetType());
        }
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

    public void OnFire1(InputValue v)
    {
        if (v.Get<float>() == 1)
        {
            //gun fire down;
            mainGun.TriggerDown();
        }
        else
        {
            //gun fire up
            mainGun.TriggerUp();

        }
    }
    public void OnFire2(InputValue v)
    {

        if (v.Get<float>() == 1)
        {
            //gun fire down;
            offGun.TriggerDown();
        }
        else
        {
            //gun fire up
            offGun.TriggerUp();
        }
    }


}
