using System;
using TMPro;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerCombatManager : CombatEntity
{
    public Transform LeftArmMount;
    public Transform RightArmMount;

    private ConstraintSource LeftConstraint;
    private ConstraintSource RightConstraint;

    private GameObject LeftWeapon;
    private GameObject RightWeapon;
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

        Weapons = new Gun[2];
        //DEBUG CODE ONLY
        AssignWeapons(1, 0);

        //Need to assign frame abilities here.
        Abilities = new Ability[0];


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
        //Remove any previous weapons.
        Destroy(LeftWeapon);
        Destroy(RightWeapon);


        //Instantiate the weapons.
        LeftWeapon = Instantiate(WeaponPrefabs[left_id]);
        RightWeapon = Instantiate(WeaponPrefabs[right_id]);

        //Now add their constraints.
        SetGunConstraints();

        //Now get scripts & set owner variables.
        Weapons[0] = LeftWeapon.GetComponent<Gun>();
        Weapons[1] = RightWeapon.GetComponent<Gun>();
        if (Weapons[0] == null || Weapons[1] == null)
        {
            throw new System.Exception("PlayerController: AssignWeapons: Missing a gun.");
        }
        (Weapons[0] as Gun).SetOwner(this);
        (Weapons[1] as Gun).SetOwner(this);

        //set UI text
        (Weapons[0] as Gun).myHudText = leftPrimaryUIText;
        (Weapons[0] as Gun).myHudSecondaryText = leftSecondaryUIText;

        (Weapons[1] as Gun).myHudText = rightPrimaryUIText;
        (Weapons[1] as Gun).myHudSecondaryText = rightSecondaryUIText;



    }
    private void SetGunConstraints()
    {
        LeftWeapon.AddComponent<ParentConstraint>().AddSource(LeftConstraint);
        LeftWeapon.GetComponent<ParentConstraint>().constraintActive = true;
        LeftWeapon.transform.localScale = new Vector3(-1, 1, 1);
        RightWeapon.AddComponent<ParentConstraint>().AddSource(RightConstraint);
        RightWeapon.GetComponent<ParentConstraint>().constraintActive = true;
    }
    private void AdjustPivotAndAimPoints()
    {
        if (player3PCam.IsDashing || player3PCam.IsSprinting) { return; }
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
            (Weapons[0] as Gun).TriggerDown();
        }
        else if (v.Get<float>() < InputSystem.settings.buttonReleaseThreshold)
        {
            //gun fire up
            (Weapons[0] as Gun).TriggerUp();

        }
    }
    public void OnFire2(InputValue v)
    {

        if (v.Get<float>() > InputSystem.settings.defaultButtonPressPoint)
        {
            //gun fire down;
            (Weapons[1] as Gun).TriggerDown();
        }
        else if (v.Get<float>() < InputSystem.settings.buttonReleaseThreshold)
        {
            //gun fire up
            (Weapons[1] as Gun).TriggerUp();
        }
    }

    public void OnReload()
    {
        (Weapons[0] as Gun).Reload();
        (Weapons[1] as Gun).Reload();
    }

    public override void OnKill(DamageInstance d)
    {
        base.OnKill(d);
        player3PCam.EnemyKilledEvent(d.Target);
    }
}
