using Godot;

public partial class Control : Godot.Control
{
    private LineEdit inputLineEdit;
    private Button sendButton;
    private ChatManager chatManager;

    public override void _Ready()
    {
        inputLineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");
        sendButton = GetNode<Button>("VBoxContainer/Button");
        chatManager = GetNode<ChatManager>("/root/Main/ChatManager"); // Adjust path as needed

        inputLineEdit.PlaceholderText = "Input your answer here";

        sendButton.Connect("pressed", new Callable(this, nameof(OnSendButtonPressed)));
    }

    private async void OnSendButtonPressed()
    {
        string userMessage = inputLineEdit.Text;
        inputLineEdit.Clear();

        chatManager.HandleUserResponse(userMessage);
    }
}
