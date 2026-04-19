

public class CharacterContext 
{
    public ICharacterMovement Movement {get ;}
    public ICharacterAnimator Animator {get ;}

    public CharacterContext(
        ICharacterMovement movement,
        ICharacterAnimator animator)
    {
        Movement = movement;
        Animator = animator;
    }
}
