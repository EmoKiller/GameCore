using UnityEngine;
namespace Game.Character.Core
{
    public interface ICharacterActions
    {
        void Move(Vector3 direction);
        Vector3 ActualVelocity();
        void SetSprint(bool value);

        void Jump();
        void SetAnimatorSpeed();
        void SetSpeedVertical();
        bool IsGrouned();
    }
    public sealed class CharacterActions : ICharacterActions
    {
        private readonly CharacterContext _core;

        public CharacterActions(CharacterContext core)
        {
            _core = core;
        }
        //===================================
        // Movement
        //===================================
        public void Move(Vector3 direction)
        {
            _core.Movement.Move(direction);
        }
        public Vector3 ActualVelocity()
        {
            return _core.Movement.ActualVelocity;
        }
        public void SetSprint(bool value)
        {
            _core.Movement.SetSprint(value);
        }

        public void Jump()
        {
            _core.Movement.Jump();
            _core.Animator.SetGrounded(false);
        }

        
        //===================================
        // Animator
        //===================================
        public void SetAnimatorSpeed()
        {
            _core.Animator.SetMoveSpeed(_core.Movement.TargetVelocityX);
        }
        public void SetSpeedVertical()
        {
            _core.Animator.SetSpeedVertical(_core.Movement.ActualVelocity.y);
        }
        //===================================
        // Sensor
        //===================================
        public bool IsGrouned()
        {
            return _core.Sensor.IsGrounded;
        }

    }
}
