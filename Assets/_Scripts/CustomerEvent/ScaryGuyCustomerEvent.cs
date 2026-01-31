
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaryGuyCustomerEvent : ICustomerEvent
{
    public void OnSpawn(Customer customer)
    {
        DOTween.To(() => RenderSettings.ambientSkyColor, x => RenderSettings.ambientSkyColor = x, new Color(0.1245283f, 0.1245283f, 0.1245283f), 5f);
        DOTween.To(() => RenderSettings.ambientEquatorColor, x => RenderSettings.ambientEquatorColor = x, new Color(0.1245283f, 0.1245283f, 0.1245283f), 5f);
        DOTween.To(() => GameManager.instance.musicSource.volume, x => GameManager.instance.musicSource.volume = x, 0f, 5f);
        
        customer.transform.position = new Vector3(1, -2, 0);
        customer.transform.DOMoveY(1.08f, 7f);
    }

    public void OnTalkStart(Customer customer)
    {
        GameManager.instance.typewriter.StartDialogue(customer.customerData.messages);
    }
    
    public void OnServed(Customer customer, bool success)
    {
        List<string> resultMessages = success ? customer.customerData.successMessages : customer.customerData.failedMessages;

        GameManager.instance.typewriter.StartDialogue(resultMessages, () =>
        {
            if (customer != null)
            {
                customer.transform.DOMoveY(-2, 7f).onComplete += () => OnLeave(customer);
            }
        });
    }
    
    public void OnLeave(Customer customer)
    {
        DOTween.To(() => RenderSettings.ambientSkyColor, x => RenderSettings.ambientSkyColor = x, Color.white, 5f);
        DOTween.To(() => RenderSettings.ambientEquatorColor, x => RenderSettings.ambientEquatorColor = x, Color.white, 5f);
        DOTween.To(() => GameManager.instance.musicSource.volume, x => GameManager.instance.musicSource.volume = x, 1f, 5f);

        customer.gameObject.SetActive(false);
        Object.Destroy(customer.gameObject);
        
        GameManager.instance.customerManager.CheckAndSpawnNextCustomer();
    }
}