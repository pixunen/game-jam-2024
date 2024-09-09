using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene NPC;
	private ChatManager chatManager;

	public override void _Ready()
	{
		chatManager = GetNode<ChatManager>("ChatManager");
		SpawnPlayers();
	}

	private void SpawnPlayers()
	{
		Random random = new();
		for (int i = 0; i < 1000; i++)
		{
			var playerInstance = (NPC)NPC.Instantiate();
			AddChild(playerInstance);

			Vector3 randomPosition = new(
				(float)(random.NextDouble() * 100 - 50),
				0,
				(float)(random.NextDouble() * 100 - 50)
			);

			playerInstance.Position = randomPosition;
			chatManager.RegisterPlayer(playerInstance);

			// Player will speak during the chatManager process
		}
	}
}
