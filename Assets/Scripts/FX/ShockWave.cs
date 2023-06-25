using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField] int pointsCount;
    [SerializeField] float maxRadius;
    [SerializeField] float speed;
    [SerializeField] float startWidth;
    [SerializeField] float force;
    [SerializeField] protected LayerMask m_WhatIsPlayer;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = pointsCount + 1;
    }
    private IEnumerator Blast()
    {
        float currentRadius = 0f;

        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            Damage(currentRadius);
            yield return null;
        }
    }

    private void Damage(float currentRadius)
    {
        Collider2D[] hittingObjects = Physics2D.OverlapCircleAll(transform.position, currentRadius, m_WhatIsPlayer);

        for (int i = 0; i < hittingObjects.Length; i++)
        {
            Rigidbody2D rb = hittingObjects[i].GetComponent<Rigidbody2D>();

            if (!rb)
                continue;
            Vector2 direction = (hittingObjects[i].transform.position - transform.position).normalized;

            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;
        
        for(int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;

            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }

    private void Update()
    {
    
    }

    private void Start()
    {
        StartCoroutine(Blast());
        StartCoroutine(Remove());
    }

    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
        
}