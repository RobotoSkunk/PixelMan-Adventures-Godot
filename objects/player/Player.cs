/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2023 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>
	Copyright (C) 2023 (Repertix)

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
	public partial class Player : CharacterBody2D, IGameObject, IGOImpulsable
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
		/// The speed to be applied to the player.
		/// </summary>
		private readonly Vector2 speed = new(144, 272);


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
		/// Friction applied to the player.
		/// </summary>
		private float friction = 0f;

		/// <summary>
		/// Acceleration input applied to the player.
		/// </summary>
		private float acceleration = 0f;

		/// <summary>
		/// The angle of the renderer.
		/// </summary>
		private float rendererAngle = 0f;


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
		/// If the gravity is inverted.
		/// </summary>
		private bool invertedGravity = false;

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
		/// Gravity force applied to the player.
		/// </summary>
		private float Gravity
		{
			get
			{
				if (invertedGravity) {
					return -Constants.Gravity;
				} else {
					return Constants.Gravity;
				}
			}
		}

		/// <summary>
		/// Player's jump force.
		/// </summary>
		private Vector2 JumpForce
		{
			get
			{
				Vector2 _speed = speed * floorNormal;

				if (invertedGravity) {
					return new Vector2(_speed.X, -_speed.Y);
				} else {
					return _speed;
				}
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

			killzoneHitbox.BodyEntered += (area) => {
				if (area is Player) {
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

			if (@event.IsActionPressed("jump")) {
				pressedJump = true;
			}

			if (@event.IsActionReleased("jump")) {
				releasedJump = true;
			}
		}

		public override void _Process(double delta)
		{
			if (isDead) {
				dustParticles.Emitting = false;
				return;
			}

			horizontalInput = Input.GetAxis("left", "right");

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

			Vector2 velocity = Velocity;

			#region Horizontal movement

			// Apply friction
			if (IsOnFloor() && horizontalInput == 0f) {
				friction = 4f;
				acceleration = 1f;
			} else {
				friction = 2f;
				acceleration = 3f;
			}

			if (Mathf.Abs(Velocity.X) > 10f) {
				velocity.X -= Mathf.Sign(Velocity.X) * friction * speed.X * (float)delta;
			} else {
				velocity.X = 0f;
			}

			// Apply horizontal input
			if (Mathf.Abs(Velocity.X) < speed.X && horizontalInput != 0f) {
				velocity.X += Mathf.Pow(WantedHorizontalSpeed * (float)delta, 2f)
								* Mathf.Sign(WantedHorizontalSpeed)
								* acceleration;
			}
			#endregion

			#region Vertical movement
			velocity.Y += Gravity * (float)delta;

			if (jumpTime > 0f && hangCount > 0f) {
				velocity.Y += JumpForce.Y;

				if (WantedHorizontalSpeed == 0f) {
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
			Velocity = new Vector2(
				Mathf.Clamp(velocity.X, -Constants.maxSpeed, Constants.maxSpeed),
				Mathf.Clamp(velocity.Y, -Constants.maxSpeed, Constants.maxSpeed)
			);

			// The man? Pixel'd
			MoveAndSlide();


			// Apply player's rotation
			if (IsOnFloor()) {
				floorNormal = GetFloorNormal();

				rendererAngle = floorNormal.Angle() + Mathf.DegToRad(90f);
			} else {
				floorNormal = Vector2.Zero;

				rendererAngle = 0f;
			}

			renderer.Rotation = Mathf.Lerp(renderer.Rotation, rendererAngle, 0.33f);
		}


		public void AddVelocity(Vector2 velocity)
		{
			Velocity += velocity;
		}

		public void Impulse(float direction, float force)
		{
			Vector2 directionVector = RSMath.AngleToVector(direction);

			Velocity *= Vector2.One - RSMath.Abs(directionVector);
			Velocity += directionVector * force;

			jumpTime = 0f;
		}

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
		}


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
