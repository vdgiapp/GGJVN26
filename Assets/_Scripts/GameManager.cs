using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Masks Data")]
    [SerializeField] private List<MaskSO> AllMasksData;
    [Header("Customer")]
    public Customer customerPrefab;
    public Transform customerPoint;
    Customer currentCustomer;
    [Header("UI")]
    public TextMeshProUGUI MessageText;
    public TextMeshProUGUI moneyText;
    public int Money = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnCustomer();
        UpdateScore(0);
    }
    void SpawnCustomer()
    {
        if (currentCustomer)
            Destroy(currentCustomer.gameObject);

        MaskSO randomMask = AllMasksData[Random.Range(0, AllMasksData.Count)];

        currentCustomer = Instantiate(customerPrefab, customerPoint.position, Quaternion.identity);
        currentCustomer.Initialize(randomMask);
    }
    public void TrySellMask(MaskSO selectedMask)
    {
        int bonus;
        if (selectedMask == currentCustomer.GetWantedMask())
        {
            MessageText.text = "Correct! You selected the right mask.";
            bonus = 10;
            // Proceed to next customer or level
            SpawnCustomer();
        }
        else
        {
            MessageText.text = "Incorrect! Try again.";
            bonus = -10;
        }
        UpdateScore(bonus);
        SpawnCustomer();
    }
    private void UpdateScore(int bonus)
    {
        Money += bonus;
        moneyText.text = Money.ToString();
    }
    private void NextMessages()
    {
        MessageText.text = currentCustomer.NextMessage();
    }

}
