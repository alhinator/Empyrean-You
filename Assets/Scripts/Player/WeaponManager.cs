using System;
using TMPro;

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
    public TMP_Text leftPrimaryUIText;
    public TMP_Text rightPrimaryUIText;
    public TMP_Text leftSecondaryUIText;
    public TMP_Text rightSecondaryUIText;

    [SerializeField] public GameObject[] WeaponPrefabs;

    [Header("AimPoint stuff")]
    public Transform aimPoint;
    public Camera realCamera;
    private Player3PCam player3PCam;
    private PlayerController playerCont;
    private HUDManager Hud;

    // Start is called before the first frame update
    void Start()
    {
        //set player reference variables
        player3PCam = GetComponent<Player3PCam>();
        Hud = GetComponent<HUDManager>();
        playerCont = GetComponent<PlayerController>();


        //Assign Constraints to their transforms.
        LeftConstraint.sourceTransform = LeftArmMount;
        LeftConstraint.weight = 1f;
        RightConstraint.sourceTransform = RightArmMount;
        RightConstraint.weight = 1f;

        //DEBUG CODE ONLY
        AssignWeapons(0, 1);

        //Get their gun scripts
        mainGun = RightWeapon.GetComponent<Gun>();
        offGun = LeftWeapon.GetComponent<Gun>();
        mainGun.myHudText = leftPrimaryUIText;
        offGun.myHudText = rightPrimaryUIText;
        mainGun.myHudSecondaryText = leftSecondaryUIText;
        offGun.myHudSecondaryText = rightSecondaryUIText;
        mainGun.playerReference = playerCont;
        offGun.playerReference = playerCont;
        if (mainGun == null || offGun == null)
        {
            throw new System.Exception("PlayerController: Start: Missing a gun.");
        }
        else
        {
            Debug.Log(offGun.GetType());
        }

        
    }
    void Update()
    {

        float furthest = Math.Max(LeftWeapon.GetComponent<Gun>().range, RightWeapon.GetComponent<Gun>().range);
        Vector3 direction = (Hud.reticle.transform.position - realCamera.transform.position).normalized;
        Physics.Raycast(realCamera.transform.position, direction, out RaycastHit hit, furthest, LayerMask.GetMask("WalkableTerrain", "CameraObstacle", "Enemy"));
        if (hit.transform)
        {
            aimPoint.position = hit.point;
        }
        else
        {
            aimPoint.position = realCamera.transform.position + (direction * furthest);
        }

        AdjustPivotAndAimPoints();


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
        LeftWeapon.transform.localScale = new Vector3(-1, 1, 1);
        RightWeapon.AddComponent<ParentConstraint>().AddSource(RightConstraint);
        RightWeapon.GetComponent<ParentConstraint>().constraintActive = true;

        LeftWeapon.GetComponent<Gun>().weaponManager = this;
        RightWeapon.GetComponent<Gun>().weaponManager = this;

    }
    private void AdjustPivotAndAimPoints()
    {
        Transform lpp = LeftWeapon.GetComponent<Gun>().pivotPoint;
        Transform rpp = RightWeapon.GetComponent<Gun>().pivotPoint;
        lpp.forward = Vector3.Lerp(lpp.forward, aimPoint.position - lpp.position, Time.deltaTime).normalized;
        rpp.forward = Vector3.Lerp(rpp.forward, aimPoint.position - rpp.position, Time.deltaTime).normalized;

        Transform lbp = LeftWeapon.GetComponent<Gun>().bulletOrigin;
        Transform rbp = RightWeapon.GetComponent<Gun>().bulletOrigin;
        lbp.forward = (aimPoint.position - lbp.position).normalized;
        rbp.forward = (aimPoint.position - rbp.position).normalized;
    }

    public void OnFire1(InputValue v)
    {
        if (v.Get<float>() > InputSystem.settings.defaultButtonPressPoint)
        {
            //gun fire down;
            mainGun.TriggerDown();
        }
        else if (v.Get<float>() < InputSystem.settings.buttonReleaseThreshold)
        {
            //gun fire up
            mainGun.TriggerUp();

        }
    }
    public void OnFire2(InputValue v)
    {

        if (v.Get<float>() > InputSystem.settings.defaultButtonPressPoint)
        {
            //gun fire down;
            offGun.TriggerDown();
        }
        else if (v.Get<float>() < InputSystem.settings.buttonReleaseThreshold)
        {
            //gun fire up
            offGun.TriggerUp();
        }
    }


}
