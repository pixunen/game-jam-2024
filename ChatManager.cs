using System.Collections.Generic;
using Godot;

public partial class ChatManager : Node
{
    private Queue<NPC> entities = new();
    private bool isRunning = false;
    private NPC currentEntity;
    private Camera3D camera;
    private Control control;
    private Button restartButton;
    private Label endGameLabel;

    public override void _Ready()
    {
        camera = GetNode<Camera3D>("../Camera3D"); // Adjust the path to your Camera3D node
        control = GetNode<Control>("../Control");
        restartButton = GetNode<Button>("../RestartButton");
        endGameLabel = GetNode<Label>("../EndGameLabel");

        if (camera == null)
        {
            GD.PrintErr("Camera3D node not found");
        }

        if (restartButton == null)
        {
            GD.PrintErr("RestartButton node not found");
        }
        else
        {
            restartButton.Connect("pressed", new Callable(this, nameof(OnRestartButtonPressed)));
        }

        if (endGameLabel == null)
        {
            GD.PrintErr("EndGameLabel node not found");
        }
        else
        {
            endGameLabel.Visible = false; // Ensure the label is initially hidden
            endGameLabel.MouseFilter = Control.MouseFilterEnum.Ignore; // Ignore mouse events
        }
    }

    public void RegisterPlayer(NPC entity)
    {
        entities.Enqueue(entity);
        entity.Connect("FinishedSpeakingEventHandler", new Callable(this, nameof(OnPlayerFinishedSpeaking)));
        if (!isRunning)
        {
            isRunning = true;
            StartNextPlayer();
        }
    }

    private void StartNextPlayer()
    {
        if (entities.Count > 0)
        {
            currentEntity = entities.Dequeue();
            MoveNpcToFrontOfCamera(currentEntity);
            currentEntity.Speak();
        }
        else
        {
            isRunning = false;
            GD.Print("All NPCs have been answered. Ending the game.");
            ShowEndGameMessage("You answered correctly to all NPCs");
        }
    }

    private void OnPlayerFinishedSpeaking(bool response)
    {
        // Waiting for the user to respond, do nothing here.
    }

    private async void MoveNpcToFrontOfCamera(NPC npc)
    {
        if (camera == null) return;

        Vector3 cameraPosition = camera.GlobalTransform.Origin;
        Vector3 forward = -camera.GlobalTransform.Basis.Z; // Assuming the camera looks along the -z axis
        Vector3 targetPosition = cameraPosition + forward * 5.0f; // Move 5 units in front of the camera

        var tween = CreateTween();
        tween.TweenProperty(npc, "global_transform:origin", targetPosition, 1.0f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        await ToSignal(tween, "finished");
    }

    public async void HandleUserResponse(string userMessage)
    {
        if (currentEntity == null) return;

        bool isResponseCorrect = await currentEntity.InteractAndGetResponse(userMessage);

        if (isResponseCorrect)
        {
            await currentEntity.Despawn();  // Despawn the player
            StartNextPlayer();
        }
        else
        {
            ShowEndGameMessage("You answered WRONG!");
        }
    }

    private void ShowEndGameMessage(string message)
    {
        if (endGameLabel == null) return;

        endGameLabel.Text = message;
        endGameLabel.Visible = true; // Show the end game label
        endGameLabel.MouseFilter = Control.MouseFilterEnum.Ignore; // Ensure it ignores mouse events

        restartButton.Visible = true;  // Show the restart button
    }

    private void OnRestartButtonPressed()
    {
        GD.Print("Restart button pressed.");
        GetTree().Paused = false; // Unpause the game if paused

        // Reload the main scene
        Error result = GetTree().ChangeSceneToFile("res://Main.tscn");
        if (result != Error.Ok)
        {
            GD.PrintErr("Failed to change scene. Error: " + result);
        }
        else
        {
            GD.Print("Scene change successful.");
        }
    }
}
