using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Masks Data")]
    [SerializeField] private List<CustomerSO> customerSOs;
    [Header("Customer")]
    public Customer normalCustomerPrefab;
    // Optional: assign a special customer prefab (can have different visuals/behaviour)
    public Customer specialCustomerPrefab;
    public Transform customerPoint;
    // spacing (world units) between duo customers
    public float duoSpacing = 1.5f;

    Customer currentCustomer;
    Customer LastestCustomer;

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
        SpawnCustomerNormal();
        UpdateScore(0);
    }
    #region Customer Spawning

    void SpawnCustomer(CustomerType customerType)
    {
        switch (customerType)
        {
            case CustomerType.Normal:
                SpawnCustomerNormal();
                break;
            case CustomerType.VIP:
                SpawnCustomerSpecial();
                break;
            case CustomerType.FollowUp:
                SpawnCustomerFollowUp();
                break;
            case CustomerType.Duo:
                SpawnCustomerDuo();
                break;
            default:
                SpawnCustomerNormal();
                break;
        }

    }

    private void ClearExistingCustomers()
    {
        // Destroy any remaining customers (including LastestCustomer) to ensure a clean spawn state.
        if (currentCustomer != null)
        {
            Destroy(currentCustomer.gameObject);
            currentCustomer = null;
        }
        if (LastestCustomer != null)
        {
            Destroy(LastestCustomer.gameObject);
            LastestCustomer = null;
        }
    }

    private void SpawnCustomerNormal()
    {
        ClearExistingCustomers();

        // Select a random normal customer SO
        CustomerSO customerSOTemp = null;
        foreach (CustomerSO customerSO in customerSOs)
        {
            if (customerSO.type == CustomerType.Normal)
            {
                customerSOTemp = customerSO;
                break;
            }
        }

        if (customerSOTemp == null)
        {
            // fallback: take any defined SO
            if (customerSOs.Count > 0) customerSOTemp = customerSOs[0];
            else return;
        }

        currentCustomer = Instantiate(normalCustomerPrefab, customerPoint.position, Quaternion.identity);
        currentCustomer.Initialize(customerSOTemp);
        MessageText.text = currentCustomer.NextMessage();
    }

    private void SpawnCustomerSpecial()
    {
        ClearExistingCustomers();

        // Select a random VIP customer SO
        CustomerSO customerSOTemp = null;
        foreach (CustomerSO customerSO in customerSOs)
        {
            if (customerSO.type == CustomerType.VIP)
            {
                customerSOTemp = customerSO;
                break;
            }
        }

        if (customerSOTemp == null)
        {
            // fallback
            if (customerSOs.Count > 0) customerSOTemp = customerSOs[0];
            else return;
        }

        currentCustomer = Instantiate(specialCustomerPrefab, customerPoint.position, Quaternion.identity);
        currentCustomer.Initialize(customerSOTemp);
        MessageText.text = currentCustomer.NextMessage();
    }

    private void SpawnCustomerFollowUp()
    {
        // FollowUp mechanic:
        // - If there is no stored LastestCustomer (previous FollowUp first), spawn the "IsFirst" FollowUp SO and store it as LastestCustomer.
        // - If LastestCustomer exists, use its wanted mask as the forced mask for the follow-up customer (IsFirst == false),
        //   destroy the previous LastestCustomer GameObject and spawn the follow-up customer that requests the same mask.

        // If there is no LastestCustomer, clear other customers and spawn the first follow up
        if (LastestCustomer == null)
        {
            ClearExistingCustomers();

            CustomerSO customerSOTemp = null;
            foreach (CustomerSO customerSO in customerSOs)
            {
                if (customerSO.type == CustomerType.FollowUp && customerSO.IsFirst)
                {
                    customerSOTemp = customerSO;
                    break;
                }
            }
            // fallback: any FollowUp SO
            if (customerSOTemp == null)
            {
                foreach (CustomerSO customerSO in customerSOs)
                {
                    if (customerSO.type == CustomerType.FollowUp)
                    {
                        customerSOTemp = customerSO;
                        break;
                    }
                }
            }

            if (customerSOTemp == null)
            {
                // No follow-up SO defined, fallback to normal spawn
                SpawnCustomerNormal();
                return;
            }

            currentCustomer = Instantiate(normalCustomerPrefab, customerPoint.position, Quaternion.identity);
            currentCustomer.Initialize(customerSOTemp);
            // store this as the "first" in the follow-up chain so the next FollowUp spawn can reference its mask
            LastestCustomer = currentCustomer;
            MessageText.text = currentCustomer.NextMessage();
        }
        else
        {
            // There is a previous customer that started a follow-up chain — use its mask for this follow-up
            MaskSO forcedMask = LastestCustomer.GetWantedMask();

            // destroy previous starter now that we grabbed the needed mask
            Destroy(LastestCustomer.gameObject);
            LastestCustomer = null;

            // find a FollowUp SO that is not marked IsFirst (preferred)
            CustomerSO customerSOTemp = null;
            foreach (CustomerSO customerSO in customerSOs)
            {
                if (customerSO.type == CustomerType.FollowUp && !customerSO.IsFirst)
                {
                    customerSOTemp = customerSO;
                    break;
                }
            }
            // fallback to any FollowUp SO
            if (customerSOTemp == null)
            {
                foreach (CustomerSO customerSO in customerSOs)
                {
                    if (customerSO.type == CustomerType.FollowUp)
                    {
                        customerSOTemp = customerSO;
                        break;
                    }
                }
            }

            if (customerSOTemp == null)
            {
                // If still none, fallback to normal
                SpawnCustomerNormal();
                return;
            }

            currentCustomer = Instantiate(normalCustomerPrefab, customerPoint.position, Quaternion.identity);
            currentCustomer.Initialize(customerSOTemp, forcedMask);
            MessageText.text = currentCustomer.NextMessage();
        }
    }

    private void SpawnCustomerDuo()
    {
        // Duo mechanic:
        // spawn two customers side-by-side, pair them via Partner reference.
        // player serves one, then the other without advancing to next event until both are served.

        ClearExistingCustomers();

        CustomerSO duoSO = null;
        foreach (CustomerSO customerSO in customerSOs)
        {
            if (customerSO.type == CustomerType.Duo)
            {
                duoSO = customerSO;
                break;
            }
        }

        if (duoSO == null)
        {
            // no duo SO defined -> fallback
            SpawnCustomerNormal();
            return;
        }

        // pick two masks (distinct if possible)
        MaskSO maskA = null;
        MaskSO maskB = null;
        int count = duoSO.Masks.Count;
        if (count == 0)
        {
            // fallback to any mask from any SO
            foreach (var so in customerSOs)
            {
                if (so.Masks.Count > 0)
                {
                    maskA = so.Masks[0];
                    break;
                }
            }
            maskB = maskA;
        }
        else if (count == 1)
        {
            maskA = duoSO.Masks[0];
            maskB = duoSO.Masks[0];
        }
        else
        {
            int idxA = Random.Range(0, count);
            int idxB = Random.Range(0, count);
            // ensure distinct if possible
            if (idxA == idxB)
            {
                idxB = (idxA + 1) % count;
            }
            maskA = duoSO.Masks[idxA];
            maskB = duoSO.Masks[idxB];
        }

        Vector3 leftPos = customerPoint.position + Vector3.left * (duoSpacing / 2f);
        Vector3 rightPos = customerPoint.position + Vector3.right * (duoSpacing / 2f);

        Customer left = Instantiate(normalCustomerPrefab, leftPos, Quaternion.identity);
        Customer right = Instantiate(normalCustomerPrefab, rightPos, Quaternion.identity);

        left.Initialize(duoSO, maskA);
        right.Initialize(duoSO, maskB);

        left.Partner = right;
        right.Partner = left;

        left.Served = false;
        right.Served = false;

        currentCustomer = left;
        // store the other so we can clear later if needed
        LastestCustomer = right;

        MessageText.text = currentCustomer.NextMessage();
    }
    #endregion
    #region Mechanic Check Mask
    public void TrySellMask(MaskSO selectedMask)
    {
        int bonus;
        if (selectedMask == currentCustomer.GetWantedMask())
        {
            MessageText.text = "Correct! You selected the right mask.";
            bonus = 10;
        }
        else
        {
            MessageText.text = "Incorrect! Try again.";
            bonus = -10;
        }
        UpdateScore(bonus);

        // mark current as served
        currentCustomer.Served = true;

        // If this customer has a partner (Duo) and the partner hasn't been served yet,
        // switch currentCustomer to the partner and show its message instead of spawning next event.
        if (currentCustomer.Partner != null && !currentCustomer.Partner.Served)
        {
            currentCustomer = currentCustomer.Partner;
            MessageText.text = currentCustomer.NextMessage();
            return;
        }

        // No pending partner: proceed to next event + cleanup
        LastestCustomer = null;
        SpawnCustomer(EventManager.Instance.NextEvent());
    }
    #endregion
    #region UI Handlers
    private void UpdateScore(int bonus)
    {
        Money += bonus;
        moneyText.text = Money.ToString();
    }
    private void NextMessages()
    {
        if(currentCustomer != null)
        {
            string message = currentCustomer.NextMessage();
            if (message != null)
            {
                MessageText.text = message;
            }
        }
    }
    #endregion
}
