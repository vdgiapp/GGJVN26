public interface IInteractable
{
    // Allows interactables to decide if they are currently interactable
    public bool IsCurrentlyInteractable();

    // Allows the interactable to decide what text to appear when the player looks at it
    public string LookAtText();

    // What happens when the player interacts with this interactable?
    public void Interact();
}