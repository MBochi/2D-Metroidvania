using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpToSafeGround : MonoBehaviour
{
    [SerializeField] private float saveFrequency = 3f;

    public Vector2 SafeGroundLocation {get; private set;} = Vector2.zero; // same as new Vector2(0f, 0f);

    private Coroutine safeGroundCoroutine;
    private CharacterController2D cc;

    private void Start() 
    {
        cc = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());

        SafeGroundLocation = transform.position;
    }

    private IEnumerator SaveGroundLocation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < saveFrequency)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(cc.m_Grounded)
        {
            SafeGroundLocation = transform.position;
        }
        
        safeGroundCoroutine = StartCoroutine(SaveGroundLocation());
    }

    public void WarpPlayerToSafeGround()
    {
        transform.position = SafeGroundLocation;
    }
}
