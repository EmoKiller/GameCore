using UnityEngine;

public class InputBuffer : MonoBehaviour
{
    [SerializeField] private float bufferTime = 0.2f;

    private float attackBuffer;
    private float jumpBuffer;

    private void Update()
    {
        attackBuffer -= Time.deltaTime;
        jumpBuffer -= Time.deltaTime;
    }

    public void BufferAttack()
    {
        attackBuffer = bufferTime;
    }

    public void BufferJump()
    {
        jumpBuffer = bufferTime;
    }

    public bool ConsumeAttack()
    {
        if (attackBuffer > 0f)
        {
            attackBuffer = 0f;
            return true;
        }

        return false;
    }

    public bool ConsumeJump()
    {
        if (jumpBuffer > 0f)
        {
            jumpBuffer = 0f;
            return true;
        }

        return false;
    }
}
