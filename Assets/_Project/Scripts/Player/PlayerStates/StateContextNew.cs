using UnityEngine;

public class StateContextNew
{
    public float CurrentHealth { get; set; }
    public int WeaponID { get; set; }
    public GameObject CurrentGunObject { get; set; }
    public Gun CurrentGunScript { get; set; }

    public bool isDamaged { get; set; } 
    public int RunAnimHash { get; set; }
    public int RunBackwardsAnimHash { get; set; }
    public int DamageAnimHash{ get; set; }
    public int DeathAnimHash { get; set; }
    public int IdleAnimHash { get; set; }

}
