using System;
using UnityEngine;

public class WaitingForGameToStartWindow : MonoBehaviour
{
    private void Start()
    {
        Bird.Instance.OnStart += Bird_OnStart;
    }

    private void Bird_OnStart(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void HideFlags()
    {
        throw new NotImplementedException();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
