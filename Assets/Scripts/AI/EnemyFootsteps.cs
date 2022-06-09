using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFootsteps : MonoBehaviour
{
    [SerializeField] float stepSpeed;
    private float GetStepOffset => stepSpeed;


    private float footstepTimer;
    private int footstepIndex;

    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioClip[] defaultSteps;
    [SerializeField] AudioClip[] woodSteps;
    [SerializeField] AudioClip[] metalSteps;
    [SerializeField] AudioClip[] jumpLandings;
    [SerializeField] AudioClip[] metalLandings;

    private EnemyAI controller;
    [SerializeField] LayerMask ground;

    void Start()
    {
        footstepIndex = 0;

        controller = GetComponent<EnemyAI>();
        //footstepSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!controller.isDead && controller.isWalking)
        {
            Footstep();
        }
    }

    private void Footstep()
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(controller.transform.position, Vector3.down, out RaycastHit hit, 2, ground))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/WOOD":
                        if (footstepIndex > woodSteps.Length - 1)
                        {
                            footstepIndex = 0;
                            footstepSource.PlayOneShot(woodSteps[footstepIndex], 0.5f);
                            footstepIndex++;
                        }
                        else
                        {
                            footstepSource.PlayOneShot(woodSteps[footstepIndex], 0.5f);
                            footstepIndex++;
                        }

                        break;
                    case "Footsteps/METAL":
                        if (footstepIndex > metalSteps.Length - 1)
                        {
                            footstepIndex = 0;
                            footstepSource.PlayOneShot(metalSteps[footstepIndex], 0.5f);
                            footstepIndex++;
                        }
                        else
                        {
                            footstepSource.PlayOneShot(metalSteps[footstepIndex], 0.5f);
                            footstepIndex++;
                        }
                        break;
                    /*case "Footsteps/CONCRETE":
                        if (footstepIndex > woodSteps.Length - 1)
                        {
                            footstepIndex = 0;
                            footstepSource.PlayOneShot(defaultSteps[footstepIndex]);
                            footstepIndex++;
                        }
                        else
                        {
                            footstepSource.PlayOneShot(defaultSteps[footstepIndex]);
                            footstepIndex++;
                        }
                        break;*/
                    default:
                        if (footstepIndex > defaultSteps.Length - 1)
                        {
                            footstepIndex = 0;
                            footstepSource.PlayOneShot(defaultSteps[footstepIndex], 0.5f);
                            footstepIndex++;
                        }
                        else
                        {
                            footstepSource.PlayOneShot(defaultSteps[footstepIndex], 0.5f);
                            footstepIndex++;
                        }
                        break;
                }

                footstepTimer = GetStepOffset;

            }
        }
    }
}
