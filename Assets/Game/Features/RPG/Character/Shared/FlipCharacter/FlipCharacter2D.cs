using UnityEngine;
public interface IFlipCharacter 
{
    void HandleFacing(Vector2 dir);
}
public class FlipCharacter2D : IFlipCharacter
{
    private Transform _target;
    protected bool facingRight = true;

    public FlipCharacter2D(Transform target)
    {
        _target = target;
    }

    public virtual void HandleFacing(Vector2 dir)
    {
        if (_target == null) return;

        if (dir.x > 0 && !facingRight)
        {
            SetFacing(true);
        }
        else if (dir.x < 0 && facingRight)
        {
            SetFacing(false);
        }
    }

    private void SetFacing(bool right)
    {
        facingRight = right;

        _target.localScale = new Vector3(right ? 1 : -1, 1, 1);
    }

}
