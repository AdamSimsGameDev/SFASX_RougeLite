using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float SingleNodeMoveTime = 0.5f;

    public EnvironmentTile currentPosition { get; set; }
    public bool IsAnimating;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        // we can tell whether we are animating based on the current animation state.
        // if the state is our 'Idle' animation, we know that we aren't doing any important animations.
        IsAnimating = !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    public void Damage(float damage)
    {
        Debug.Log("Enemy took " + damage + " damage!");
        animator.SetTrigger("Hit");
    }

    public void LookAt(Vector3 position)
    {
        transform.rotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
    }
}
