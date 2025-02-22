using UnityEngine;
public class Itzi : Ability
{
    [Header("Ability-specific variables")]
    [SerializeField] int adjDashes = 8;
    [SerializeField] float adjDashForce = 30f;
    [SerializeField] float adjHoverRate = 0.2f;
    [SerializeField] float adjAirDrag = 1.5f;
    [SerializeField] float adjAirJumpForce = 15f;


    private Player3PCam player;
    void Start()
    {
        player = Owner.GetComponent<Player3PCam>();
        player.maxMidairBoosts = adjDashes;
        player.hoverMultiplier = adjHoverRate;
        player.airDrag = adjAirDrag;
        player.airJumpForce = adjAirJumpForce;
        player.dashForce = adjDashForce;
    }
    public override void OnKill(DamageInstance d)
    {
        base.OnKill(d);
        player.RecoverADash();
    }
}