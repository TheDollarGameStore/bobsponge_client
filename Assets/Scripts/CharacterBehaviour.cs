using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    IDLING,
    WALKING,
    TALKING
}

public class CharacterBehaviour : MonoBehaviour
{
    private Animator animator;

    [HideInInspector] public CharacterState characterState;

    [SerializeField] private float minStateTime;
    [SerializeField] private float maxStateTime;

    [SerializeField] private float walkSpeed;

    private Vector2 destination;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Invoke("ChangeState", Random.Range(minStateTime, maxStateTime));
    }

    // Update is called once per frame
    void ChangeState()
    {
        if (characterState == CharacterState.IDLING)
        {
            characterState = CharacterState.WALKING;
            animator.SetBool("walking", true);
            animator.SetBool("idling", false);
            animator.SetBool("talking", false);

            destination = new Vector2(Random.Range(5.15f, 6.15f), Random.Range(-1.4f, -2.5f));
        }
        else if (characterState == CharacterState.WALKING)
        {
            characterState = CharacterState.IDLING;
            animator.SetBool("walking", false);
            animator.SetBool("idling", true);
            animator.SetBool("talking", false);
        }

        Invoke("ChangeState", Random.Range(minStateTime, maxStateTime));
    }

    private void Update()
    {
        if (characterState == CharacterState.WALKING)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(destination.x, transform.position.y, destination.y), walkSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(destination.x, transform.position.y, destination.y) - transform.position, Vector3.up), 10f * Time.deltaTime);

            if (Vector3.Distance(transform.position, new Vector3(destination.x, transform.position.y, destination.y)) <= 0.01f)
            {
                CancelInvoke("ChangeState");
                ChangeState();
            }
        }
    }
}
