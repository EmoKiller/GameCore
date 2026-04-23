

public interface ICharacterAnimator 
{
    void SetMoveSpeed(float speed);
    void SetSpeedVertical(float speed);
    void SetGrounded(bool grounded);
    void SetCombo(int combo);
    void TriggerAttack();
    void ResetAttack();
}
