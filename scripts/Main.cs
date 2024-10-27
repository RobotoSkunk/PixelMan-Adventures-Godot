using Godot;
using System;

public partial class Main : Node
{
	[Export] private RichTextLabel debugLabel;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		debugLabel.Text = "FPS: " + Engine.GetFramesPerSecond() +
						"\nMemory: " + OS.GetStaticMemoryUsage() +
						"\nDraw Calls: " + Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);
	}
}
