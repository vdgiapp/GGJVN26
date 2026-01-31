using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _customerPrefabs;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private Transform _buyPosition;
    [SerializeField] private float _moveDuration = 7f;
    [SerializeField] private CustomerEventRegistry _eventRegistry;
    
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

        ICustomerEvent e = _eventRegistry.GetEvent(_currentCustomer.customerData.customerEvent);
        if (e != null)
        {
            // Perform special event spawn
            e?.OnSpawn(_currentCustomer);
        }
        else
        {
            // Move in normal
            _currentCustomer.transform.DOMoveX(_buyPosition.position.x, _moveDuration);
        }
    }
    
    public void SpawnNextCustomer()
    {
        _currentCustomerIndex++;
        SpawnCustomer(_currentCustomerIndex);
    }
    
    public void TalkToCustomer(Customer customer)
    {
        if (customer == null) return;

        ICustomerEvent e = _eventRegistry.GetEvent(_currentCustomer.customerData.customerEvent);
        if (e != null)
        {
            // Perform special event on talk start
            e?.OnTalkStart(_currentCustomer);
        }
        else
        {
            // Start dialogue normal
            GameManager.instance.typewriter.StartDialogue(customer.customerData.messages);
        }
    }
    
    public void GiveMaskToCustomer(Customer customer, bool success)
    {
        if (customer == null) return;

        Debug.Log($"Customer {customer.name} gave mask: {success}");

        customer.isServed = true;

        if (success) _successCount++;
        else _failedCount++;
        
        ICustomerEvent e = _eventRegistry.GetEvent(_currentCustomer.customerData.customerEvent);
        if (e != null)
        {
            e?.OnServed(_currentCustomer, success);
        }
        else
        {
            List<string> resultMessages = success ? customer.customerData.successMessages : customer.customerData.failedMessages;

            // chạy hội thoại kết quả -> xong mới xử lý customer
            GameManager.instance.typewriter.StartDialogue(resultMessages, () =>
            {
                // destroy customer sau khi đọc xong
                if (customer != null)
                {
                    _currentCustomer.transform.DOMoveX(_spawnPosition.position.x, _moveDuration).onComplete += () =>
                    {
                        ICustomerEvent e = _eventRegistry.GetEvent(_currentCustomer.customerData.customerEvent);
                        e?.OnLeave(_currentCustomer);

                        customer.gameObject.SetActive(false);
                        Destroy(customer.gameObject);
                        
                        CheckAndSpawnNextCustomer();
                    };
                }
            });
        }
    }

    public void CheckAndSpawnNextCustomer()
    {
        if (_currentCustomerIndex + 1 < _customerPrefabs.Count)
        {
            SpawnNextCustomer();
        }
        else
        {
            GameManager.instance.GameOver();
            GameManager.instance.ui.SetResultCount(_successCount, _customerPrefabs.Count);
        }
    }
}