using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] float stepSpeed;
    [SerializeField] float crouchStepMultiplier;
    private float GetStepOffset => controller.isCrouching ? stepSpeed * crouchStepMultiplier : stepSpeed;

    private float footstepTimer;
    private int footstepIndex;

    [SerializeField] AudioSource footstepSource;
    [SerializeField] AudioClip[] defaultSteps;
    [SerializeField] AudioClip[] woodSteps;
    [SerializeField] AudioClip[] metalSteps;
    [SerializeField] AudioClip[] jumpLandings;
    [SerializeField] AudioClip[] metalLandings;

    private CustomCharacterController controller;

    void Start()
    {
        footstepIndex = 0;

        controller = GetComponent<CustomCharacterController>();
        //footstepSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!controller.isDead && controller.isGrounded && controller.isWalking)
        {
            Footstep();
        }
    }

    private void Footstep()
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(controller.transform.position, Vector3.down, out RaycastHit hit, 2))
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
                            footstepSource.PlayOneShot(metalSteps[footstepIndex]);
                            footstepIndex++;
                        }
                        else
                        {
                            footstepSource.PlayOneShot(metalSteps[footstepIndex]);
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
                            footstepSource.PlayOneShot(defaultSteps[footstepIndex]);
                            footstepIndex++;
                        }
                        else
                        {
                            footstepSource.PlayOneShot(defaultSteps[footstepIndex]);
                            footstepIndex++;
                        }
                        break;
                }

                footstepTimer = GetStepOffset;
            }
        }
    }

    public void PlayLandingNoise()
    {
        if (Physics.Raycast(controller.transform.position, Vector3.down, out RaycastHit hit, 2))
        {
            switch (hit.collider.tag)
            {
                case "Footsteps/WOOD":
                    footstepSource.PlayOneShot(jumpLandings[0], 0.5f);
                    break;
                case "Footsteps/METAL":
                    footstepSource.PlayOneShot(metalLandings[Random.Range(0, metalLandings.Length - 1)], 0.5f);
                    break;
                default:
                    footstepSource.PlayOneShot(jumpLandings[0], 0.5f);
                    break;
            }
        }
    }
}
