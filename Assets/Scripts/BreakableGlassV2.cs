using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGlassV2 : MonoBehaviour
{
    public AudioSource audioSource;
    private GameManager manager;

    [SerializeField] private GameObject intactGlass;
    [SerializeField] private List<GameObject> glassCells;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        manager = FindObjectOfType<GameManager>();


        for (int i = 1; i < transform.childCount; i++)
        {
            glassCells.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            Break();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 6)
        {
            if (manager.matrixMode)
                Break();
        }
    }

    public void Break()
    {
        Debug.Log("glass should break");

        if (!audioSource.isPlaying)
            audioSource.Play();

        intactGlass.SetActive(false);

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            glassCells[i].SetActive(true);
            glassCells[i].GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
