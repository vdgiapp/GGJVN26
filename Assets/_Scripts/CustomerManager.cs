using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _customerPrefabs;
    [SerializeField] private Transform _spawnPosition;
    
    private int _currentCustomerIndex;
    private int _successCount;
    private int _failedCount;
    
    private Customer _currentCustomer;
    
    public int GetSuccessCount() => _successCount;
    public int GetFailedCount() => _failedCount;
    public int GetCurrentCustomerIndex => _currentCustomerIndex;
    public Customer GetCurrentCustomer => _currentCustomer;
    
    public void SpawnCustomer(int index)
    {
        if (index >= _customerPrefabs.Count)
        {
            Debug.Log("No more customers!");
            return;
        }

        GameObject customerObject = Instantiate(_customerPrefabs[index], _spawnPosition.position, Quaternion.identity);
        _currentCustomer = customerObject.GetComponent<Customer>();

        // Move in
        _currentCustomer.transform.DOMoveX(0.0f, 5f);
    }
    
    public void SpawnNextCustomer()
    {
        _currentCustomerIndex++;
        SpawnCustomer(_currentCustomerIndex);
    }
    
    public void TalkToCustomer(Customer customer)
    {
        if (customer == null) return;

        // dialogue bình thường
        GameManager.instance.typewriter.StartDialogue(customer.customerData.messages);
    }
    
    public void GiveMaskToCustomer(Customer customer, bool success)
    {
        if (customer == null) return;

        Debug.Log($"Customer {customer.name} gave mask: {success}");

        customer.isServed = true;

        if (success) _successCount++;
        else _failedCount++;

        List<string> resultMessages = success ? customer.customerData.successMessages : customer.customerData.failedMessages;

        // chạy hội thoại kết quả -> xong mới xử lý customer
        GameManager.instance.typewriter.StartDialogue(resultMessages, () =>
        {
            // destroy customer sau khi đọc xong
            if (customer != null)
            {
                customer.gameObject.SetActive(false);
                Destroy(customer.gameObject);
            }

            // spawn tiếp
            if (_currentCustomerIndex + 1 < _customerPrefabs.Count)
            {
                SpawnNextCustomer();
            }
            else
            {
                Debug.Log("End customers!");
                Debug.Log("Total success: " + _successCount);
                Debug.Log("Total failed: " + _failedCount);
            }
        });
    }
}