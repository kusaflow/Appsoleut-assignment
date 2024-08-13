using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initMenuAfterDelay : MonoBehaviour
{
    public GameObject menu_init;
    // Start is called before the first frame update
    void Start()
    {
        menu_init.SetActive(false);
        StartCoroutine(initAfterDelay());
    }

    IEnumerator initAfterDelay()
    {
        yield return new WaitForSeconds(1);
        menu_init.SetActive(true);
    }

}
