using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLocker : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private bool isOpen;
    [SerializeField]
    private bool isClosed;

    private InteractableButton button;

    void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponentInChildren<InteractableButton>();
    }

    public void OperateLocker()
    {
        if (isOpen)
            CloseLocker();
        else
            OpenLocker();
    }

    void OpenLocker()
    {
        animator.SetTrigger("Open");

        isOpen = true;
        isClosed = false;

        button.customString = "Close locker";
    }

    void CloseLocker()
    {
        animator.SetTrigger("Close");

        isClosed = true;
        isOpen = false;

        button.customString = "Open locker";
    }
}
