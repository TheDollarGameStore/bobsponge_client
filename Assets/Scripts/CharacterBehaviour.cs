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
    [SerializeField] private Animator animator;

    [HideInInspector] public CharacterState characterState;

    [SerializeField] private float minStateTime;
    [SerializeField] private float maxStateTime;

    [SerializeField] private float walkSpeed;

    [HideInInspector] public bool talking = false;

    private Vector2 destination;

    [SerializeField] private Vector2 xLimits;
    [SerializeField] private Vector2 zLimits;

    [SerializeField] private Vector3 defaultLookLocation;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ChangeState", Random.Range(minStateTime, maxStateTime));
    }

    public void ToggleTalk(bool talking)
    {
        if (this.talking && !talking)
        {
            characterState = CharacterState.IDLING;
            Invoke("ChangeState", Random.Range(minStateTime, maxStateTime));
            animator.SetBool("walking", false);
            animator.SetBool("idling", true);
            animator.SetBool("talking", false);
        }

        this.talking = talking;

        if (this.talking)
        {
            characterState = CharacterState.TALKING;

            animator.SetBool("walking", false);
            animator.SetBool("idling", false);
            animator.SetBool("talking", true);
            CancelInvoke("ChangeState");
        }
    }

    // Update is called once per frame
    void ChangeState()
    {
        if (talking)
        {
            return;
        }

        if (characterState == CharacterState.IDLING)
        {
            characterState = CharacterState.WALKING;
            animator.SetBool("walking", true);
            animator.SetBool("idling", false);
            animator.SetBool("talking", false);

            destination = new Vector2(Random.Range(xLimits.x, xLimits.y), Random.Range(zLimits.x, zLimits.y));
            Invoke("ChangeState", Random.Range(minStateTime, maxStateTime));
        }
        else if (characterState == CharacterState.WALKING)
        {
            characterState = CharacterState.IDLING;
            animator.SetBool("walking", false);
            animator.SetBool("idling", true);
            animator.SetBool("talking", false);
            Invoke("ChangeState", Random.Range(minStateTime * 2, maxStateTime * 2));
        }
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
        else if (characterState == CharacterState.TALKING)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(defaultLookLocation - transform.position, Vector3.up), 10f * Time.deltaTime);
        }
        else if (characterState == CharacterState.IDLING)
        {
            CharacterBehaviour cbTarget = ScenarioManager.instance.GetTalkingCharacter();

            if (cbTarget != null)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(cbTarget.gameObject.transform.position - transform.position, Vector3.up), 10f * Time.deltaTime);
            }
        }
    }
}
