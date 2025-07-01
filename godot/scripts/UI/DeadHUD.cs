using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class DeadHUD : CanvasLayer
{
	public VBoxContainer DebugContainer { get; set; }
	public RichTextLabel DeathTextLabel { get; set; }
	public VBoxContainer ButtonContainer { get; set; }
	public Button TryAgainButton { get; set; }
	public Button SettingButton { get; set; }
	public Button BackToMainButton { get; set; }
	public Button ExitButton { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DebugContainer = GetNode<VBoxContainer>("DebugContainer");
		DeathTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		ButtonContainer = GetNode<VBoxContainer>("ButtonContainer");
		TryAgainButton = GetNode<Button>("ButtonContainer/TryAgainButton");
		SettingButton = GetNode<Button>("ButtonContainer/SettingButton");
		BackToMainButton = GetNode<Button>("ButtonContainer/BackToMainButton");
		ExitButton = GetNode<Button>("ButtonContainer/ExitButton");

		TryAgainButton.Pressed += TryAgain;
		SettingButton.Pressed += OpenSettings;
		BackToMainButton.Pressed += BackToMainMenu;
		ExitButton.Pressed += Exit;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TryAgain()
	{

	}

	public void OpenSettings()
	{

	}

	public void BackToMainMenu()
	{

	}

	public void Exit()
	{
		GetTree().Quit();
	}
}
