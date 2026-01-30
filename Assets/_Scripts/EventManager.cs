using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    int current = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public CustomerType NextEvent()
    {
        // advance the current index and wrap around
        current++;
        int maxIndex = System.Enum.GetValues(typeof(CustomerType)).Length - 1;
        if (current > maxIndex)
        {
            current = 0;
        }
        return (CustomerType)current;
    }
}
