using Game.Application.Configuration.BaseScriptableObject;
using UnityEngine;

/// <summary>
/// Config cho Character - ScriptableObject
/// Open/Closed Principle - có thể tạo config mới mà không sửa code
/// </summary>
[CreateAssetMenu(fileName = "CharacterStatsConfig", menuName = "Game/Character Stats Config")]
public class CharacterStatsConfig : BaseConfigSo
{
    public string characterName;
    [Header("Primary Stats")]
    public float Strength = 1;
    public float Dexterity = 1;
    public float Intelligence = 1;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxStamina = 100f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 15f;

    //public PrimaryStatsData BaseStats => baseStats;
    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
}
