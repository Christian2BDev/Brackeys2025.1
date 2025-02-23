using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text timer;
    public Image[] hearts;
    public TMP_Text parts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void UpdateUI()
    {
        int i = 1;
        foreach (Image heart in hearts)
        {
            if (i <= GameController.Health)
            {
                heart.enabled = true;
                i++;
            }
            else
            {
                heart.enabled = false;
            }
        }
        
        parts.text = GameController.PartsCollected.ToString() + " / " + GameController.PartsNeeded.ToString(); 
        timer.text = TimeSpan.FromSeconds(GameController.Timer).ToString(@"mm\:ss\.ff");
    }
}
