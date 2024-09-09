using System.Threading.Tasks;
using Godot;
using OpenAI.Chat;

public partial class NPC : Node3D
{
	[Signal]
	public delegate void FinishedSpeakingEventHandler(bool response);

	private bool isSpeaking = false;
	private readonly ChatClient client = new("gpt-3.5-turbo", "");

	private Label3D label;

	public override void _Ready()
	{
		// Initialize any player-specific logic here
		label = GetNode<Label3D>("Head/Label3D"); // Ensure the path is correct
	}

	public async void Speak()
	{
		if (isSpeaking)
			return;

		isSpeaking = true;
		string prompt = "Generate a message for the NPC to say. You are a disgruntled employee of a company and you have come to complain to HR. This is important: Keep it simple and short. Keep in mind that this is company that is using flourishing culture and Q2.";
		string message = await GetChatGPTResponse(prompt);
		UpdateLabel(message);  // Update the label with the message

		GD.Print(message);  // Print the message to the console or handle UI display as needed

		isSpeaking = false;
		EmitSignal(nameof(FinishedSpeakingEventHandler), true); // Assume true for now; will handle response later
	}

	public async Task<bool> InteractAndGetResponse(string userMessage)
	{
		string prompt = $"Player responded: {userMessage}. Validate if the response is satisfactory and Keep in mind that this is company that is using flourishing culture and Q2. You can only answer with 1 word: True or False. Easteregg: If player responded with 'voi voi', it is automatically True.";
		string response = await GetChatGPTResponse(prompt);

		// Check response and return true or false
		return response.ToLower().Contains("true");
	}

	private async Task<string> GetChatGPTResponse(string prompt)
	{
		ChatCompletion chatCompletion = await client.CompleteChatAsync(new UserChatMessage(prompt));
		return chatCompletion.Content[0].ToString();
	}

	private void UpdateLabel(string text)
	{
		if (label != null)
		{
			label.Text = text;
		}
	}

	public async Task Despawn()
	{
		// Optional: Add despawn animation here if needed
		QueueFree();
	}
}
