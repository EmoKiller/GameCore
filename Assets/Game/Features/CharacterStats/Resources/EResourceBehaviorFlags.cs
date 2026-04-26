using System;

[Flags]
public enum EResourceBehaviorFlags
{
    None = 0,

    // auto regen (mana, stamina)
    Regenerates = 1 << 0,

    // trigger death
    CanBeDepleted = 1 << 1,

    // allow overheal
    AllowOvercap = 1 << 2
}