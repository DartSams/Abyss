using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public Image healthbarSPrite;
    //public TextMeshProUGUI healthText;

    private void Start()
    {
    }
    public void updateHealthbar(float maxhealth, float currenthealth)
    {
        //healthText.text = currenthealth.ToString() + "/" + maxhealth.ToString();
        healthbarSPrite.fillAmount = currenthealth / maxhealth;
    }

    public void setPosition(Vector3 offset)
    {
        //transform.position = offset;
        healthbarSPrite.transform.position = new Vector3(0,1,0)+offset;
    }

}
