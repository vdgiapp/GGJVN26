public interface ICustomerEvent
{
    void OnSpawn(Customer customer);
    void OnTalkStart(Customer customer);
    void OnServed(Customer customer, bool success);
    void OnLeave(Customer customer);
}