using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notifications : MonoBehaviour
{
    [SerializeField] private TMP_Text notification;
    [SerializeField] private GameObject notificationBox;
    [SerializeField] private float notificationFadeInTime = 2f;

    public void setText(string text)
    {
        notification.text = text;
        StartCoroutine(Notification());
    }

    public string getText()
    {
        return notification.text;
    }

    private IEnumerator Notification()
    {
        notificationBox.SetActive(true);

        for (float i = notificationFadeInTime; i >= 0; i -= Time.deltaTime)
            {
                notification.faceColor = new Color(1, 1, 1, i);
                yield return null;
            }   
        }
    }

