using UnityEngine;
public interface IFlipCharacter 
{
    void HandleFacing(Vector2 dir);
}
public class FlipCharacter2D : IFlipCharacter
{
    private GameObject _target;
    protected bool facingRight = true;

    public FlipCharacter2D(GameObject target)
    {
        _target = target;
    }

    public virtual void HandleFacing(Vector2 dir)
    {
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

        _target.transform.localScale = new Vector3(right ? 1 : -1, 1, 1);
    }

}
