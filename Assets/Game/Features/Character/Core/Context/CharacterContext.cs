

public class CharacterContext 
{
    public ICharacterMovement Movement {get ;}
    public ICharacterAnimator Animator {get ;}
    public ICharacterSensor  Sensor {get ;}

    public CharacterContext(
        ICharacterMovement movement,
        ICharacterAnimator animator,
        ICharacterSensor sensor
    )
    {
        Movement = movement;
        Animator = animator;
        Sensor = sensor;
    }
}
