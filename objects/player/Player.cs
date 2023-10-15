/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2023 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>
	Copyright (C) 2023 Repertix

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU Affero General Public License as published
	by the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Affero General Public License for more details.

	You should have received a copy of the GNU Affero General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Godot;
using Godot.Collections;

using ClockBombGames.PixelMan.Utils;
using ClockBombGames.PixelMan.Events;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class Player : DynamicBody, IGameObject
	{
		#region Variables

		[ExportGroup("Components")]
		[Export] private Node2D renderer;
		[Export] private AnimatedSprite2D animator;
		[Export] private GpuParticles2D dustParticles;
		[Export] private GpuParticles2D fallDustParticles;
		[Export] private AudioStreamPlayer2D audioPlayer;
		[Export] private CollisionShape2D collisionShape;

		[ExportGroup("Killzone detection components")]
		[Export] private Area2D killzoneHitbox;
		[Export] private CollisionShape2D killzoneCollisionShape;
		[Export] private CollisionLayers killzoneCollisionMask;

		[ExportGroup("Properties")]
		[Export] private PackedScene deathParticleScene;
		[Export(PropertyHint.ArrayType)] private AudioStream[] sounds;


		#region Readonly variables
		/// <summary>
		/// The maximum time to jump.
		/// </summary>
		private readonly float maxJumpTime = 0.16f;

		/// <summary>
		/// The maximum time to jump (minor fix for jumping near the edge).
		/// </summary>
		private readonly float maxHangCount = 0.1f;


		/// <summary>
		/// The player's death particles list.
		/// </summary>
		private readonly PlayerDeathParticle[] deathParticles = new PlayerDeathParticle[50];
		#endregion

		#region Private variables
		/// <summary>
		/// Time left to jump.
		/// </summary>
		private float jumpTime = 0f;

		/// <summary>
		/// Time left to jump (minor fix for jumping near the edge).
		/// </summary>
		private float hangCount = 0f;

		/// <summary>
		/// Horizontal input.
		/// </summary>
		private float horizontalInput = 0f;

		/// <summary>
		/// Time left to emit dust particles.
		/// </summary>
		private float dustParticlesTimer = 0f;

		/// <summary>
		/// The raw angle of the player.
		/// </summary>
		private float rawAngle = 0f;


		/// <summary>
		/// The player's index.<br/>
		/// <list type="bullet">
		///     <item>0 = Single player</item>
		///     <item>1 = Player 1</item>
		///     <item>2 = Player 2</item>
		/// </list>
		/// </summary>
		public int playerIndex = 0;


		/// <summary>
		/// If the player is in a trampoline.
		/// </summary>
		public bool isInTrampoline = false;

		/// <summary>
		/// If the jump button was pressed.
		/// </summary>
		private bool pressedJump = false;

		/// <summary>
		/// If the jump button was released.
		/// </summary>
		private bool releasedJump = false;

		/// <summary>
		/// If the jump button can reduce the jump force when released.
		/// </summary>
		private bool canReduceJump = false;

		/// <summary>
		/// If the player should emit dust particles when falling.
		/// </summary>
		private bool emitFallDustParticles = false;

		/// <summary>
		/// If the player is dead.
		/// </summary>
		private bool isDead = false;

		/// <summary>
		/// If the player is moving
		/// </summary>
		private bool isMoving = false;

		/// <summary>
		/// If the player should particles when dying.
		/// </summary>
		private bool emitDeathParticles = false;

		/// <summary>
		/// Current player state (for animation).
		/// </summary>
		private State currentState = State.IDLE;

		/// <summary>
		/// Previous player state (for animation).
		/// </summary>
		private State previousState = State.IDLE;

		/// <summary>
		/// Player's start position.
		/// </summary>
		private Vector2 startPosition;

		/// <summary>
		/// Player's previous position.
		/// </summary>
		private Vector2 previousPosition;

		/// <summary>
		/// The normal vector of the floor.
		/// </summary>
		private Vector2 floorNormal;
		#endregion

		#region Getters and setters

		/// <summary>
		/// Player's jump force.
		/// </summary>
		private Vector2 JumpForce
		{
			get
			{
				return speed * floorNormal;
			}
		}

		/// <summary>
		/// Is the player going up?
		/// </summary>
		private bool IsGoingUp
		{
			get
			{
				return Velocity.Y < 0f;
			}
		}

		/// <summary>
		/// Wanted horizontal speed.
		/// </summary>
		public float WantedHorizontalSpeed
		{
			get
			{
				return speed.X * horizontalInput;
			}
		}


		/// <summary>
		/// The player's index.
		/// </summary>
		public int PlayerIndex
		{
			get => playerIndex;
			set => playerIndex = value;
		}
		#endregion

		#endregion


		enum State
		{
			IDLE,
			RUNNING,
			JUMPING,
			FALLING,
		}


		public override void _Ready()
		{
			animator.SpriteFrames = Globals.Avatar;
			startPosition = Position;

			// Register the player
			this.RegisterPlayer();

			// Connect events
			GameEvents.OnPlayerDeath += OnPlayerDeath;
			GameEvents.OnResetGame += OnGameReset;

			killzoneHitbox.AreaEntered += (area) => {
				Globals.PlayerDied();
			};

			killzoneHitbox.BodyEntered += (body) => {
				if (body is Player) {
					return;
				}

				Globals.PlayerDied();
			};


			// Create the death particles
			for (int i = 0; i < deathParticles.Length; i++) {
				Node node = deathParticleScene.Instantiate();

				if (node is PlayerDeathParticle particle) {
					particle.Visible = false;
					deathParticles[i] = particle;
					GetOwner<Node2D>().CallDeferred("add_child", node);
				}
			}
		}

		public override void _Input(InputEvent @event)
		{
			if (isDead) {
				return;
			}


			bool isJumpPressed = playerIndex switch
			{
				1 => @event.IsActionPressed("jump_p1"),
				2 => @event.IsActionPressed("jump_p2"),
				_ => @event.IsActionPressed("jump"),
			};

			if (isJumpPressed) {
				pressedJump = true;
			}
		}

		public override void _Process(double delta)
		{
			if (isDead) {
				dustParticles.Emitting = false;
				return;
			}

			horizontalInput = playerIndex switch
			{
				1 => Input.GetAxis("left_p1", "right_p1"),
				2 => Input.GetAxis("left_p2", "right_p2"),
				_ => Input.GetAxis("left", "right"),
			};


			#region Dust Particles
			bool doDustTimer = false;

			if (IsOnFloor()) {
				doDustTimer = isMoving;

				if (emitFallDustParticles) {
					fallDustParticles.Restart();
					fallDustParticles.Emitting = true;
					emitFallDustParticles = false;
				}
			} else {
				emitFallDustParticles = true;
			}


			if (doDustTimer) {
				dustParticlesTimer = 0.08f;

			} else if (dustParticlesTimer > 0f) {
				dustParticlesTimer -= (float)delta;
			}


			renderer.Scale = new Vector2(renderer.Scale.X, invertedGravity ? -1f : 1f);

			if (horizontalInput != 0f) {
				int direction = horizontalInput > 0f ? 1 : -1;

				renderer.Scale = new Vector2(direction, renderer.Scale.Y);

				dustParticles.Emitting = dustParticlesTimer > 0f;
			} else {
				dustParticles.Emitting = false;
			}
			#endregion



			#region Animation
			if (IsOnFloor() && horizontalInput == 0f) {
				currentState = State.IDLE;

			} else if (IsOnFloor() && horizontalInput != 0f && isMoving) {
				currentState = State.RUNNING;

			} else if (!IsOnFloor() && IsGoingUp) {
				currentState = State.JUMPING;

			} else if (!IsOnFloor() && !IsGoingUp) {
				currentState = State.FALLING;

			} else {
				currentState = State.IDLE;
			}


			if (currentState != previousState) {

				string animationName = currentState switch {
					State.IDLE => "idle",
					State.RUNNING => "running",
					State.JUMPING => "jumping",
					State.FALLING => "falling",
					_ => "idle",
				};

				animator.Play(animationName);
			}

			float animationSpeed = Mathf.Abs(Velocity.X) / speed.X;

			animator.SpeedScale = currentState == State.RUNNING ? animationSpeed : 1f;
			previousState = currentState;
			#endregion
		}

		public override void _PhysicsProcess(double delta)
		{
			collisionShape.Disabled = isDead;
			killzoneHitbox.Monitoring = !isDead;
			killzoneHitbox.Monitorable = !isDead;


			if (isDead) {
				if (!emitDeathParticles) {
					emitDeathParticles = true;

					foreach (PlayerDeathParticle particle in deathParticles) {
						particle.LateVisible = true;

						particle.Freeze = false;
						particle.Reset(GlobalPosition);

						particle.LinearVelocity = RSRandom.Circle2D() * 16f * 15f;
					}
				}

				return;

			} else if (emitDeathParticles) {
				emitDeathParticles = false;

				foreach (PlayerDeathParticle particle in deathParticles) {
					particle.Visible = false;
					particle.Freeze = true;
				}
			}


			isMoving = previousPosition.DistanceSquaredTo(GlobalPosition) > 0.1f;
			previousPosition = GlobalPosition;


			if (pressedJump) {
				jumpTime = maxJumpTime;
				pressedJump = false;

			} else if (jumpTime > 0f) {
				jumpTime -= (float)delta;
			}


			// Hang Jump
			if (IsOnFloor()) {
				hangCount = maxHangCount;

			} else if (hangCount > 0f) {
				hangCount -= (float)delta;
			}

			#region Process physics

			Vector2 velocity = ProcessVelocity(WantedHorizontalSpeed, horizontalInput != 0f, (float)delta);

			#region Jump
			if (jumpTime > 0f && hangCount > 0f) {
				velocity.Y += JumpForce.Y;

				if (
					WantedHorizontalSpeed == 0f ||
					WantedHorizontalSpeed > 0f && velocity.X < JumpForce.X ||
					WantedHorizontalSpeed < 0f && velocity.X > -JumpForce.X
				) {
					velocity.X = JumpForce.X * 1.8f;
				}

				jumpTime = 0f;
				canReduceJump = true;

				audioPlayer.Stream = sounds[0];
				audioPlayer.Play();
			}

			if (isInTrampoline) {
				canReduceJump = false;

			} else if (IsGoingUp && releasedJump && canReduceJump) {
				velocity.Y *= 0.5f;
				canReduceJump = false;
			}
			#endregion

			#endregion


			if (releasedJump) {
				releasedJump = false;
			}

			// Apply changes
			ApplyVelocity(velocity);

			// The man? Pixel'd
			MoveAndSlide();


			// Apply player's rotation
			if (IsOnFloor()) {
				floorNormal = GetFloorNormal();

				float floorAngle = floorNormal.Angle();

				if (Mathf.RadToDeg(floorAngle) > 0f) {
					floorAngle -= Mathf.DegToRad(180f);
				}

				rawAngle = floorAngle + Mathf.DegToRad(90f);
			} else {
				floorNormal = Vector2.Zero;

				rawAngle = 0f;
			}

			Rotation = Mathf.Lerp(Rotation, rawAngle, 0.33f);
		}

		public override void Impulse(float direction, float force)
		{
			base.Impulse(direction, force);

			jumpTime = 0f;
		}



		#region Delegate methods
		private void OnPlayerDeath()
		{
			isDead = true;
			Velocity = Vector2.Zero;

			animator.Visible = false;

			audioPlayer.Stream = sounds[1];
			audioPlayer.Play();
		}

		private void OnGameReset()
		{
			isDead = false;
			Position = startPosition;
			Velocity = Vector2.Zero;

			animator.Visible = true;
			invertedGravity = false;
		}
		#endregion


		public Dictionary Serialize()
		{
			return new()
			{
				{ "position", Position }
			};
		}

		public void Deserialize(Dictionary data)
		{
			Position = (Vector2)data["position"];
		}
	}
}
