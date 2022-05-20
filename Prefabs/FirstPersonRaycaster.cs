using Newtonsoft.Json;
using SFML.Graphics;
using SFML.Window;
using SMPL.Graphics;
using SMPL.Tools;
using System;
using System.Collections.Generic;
using System.Numerics;
using Sprite = SMPL.Graphics.Sprite;

namespace SMPL.Prefabs
{
	public class FirstPersonRaycaster : Scene
	{
		public class Player : Object
		{
			private const float TALLNESS = 75f, DUCK_TALLNESS = 45f, DUCK_SLOW_MULTIPLIER = 0.3f;
			internal readonly Hitbox rays = new(), hitbox = new();
			private float jumpVelocity, currentTallness;

			public Keyboard.Key KeyForward { get; set; } = Keyboard.Key.W;
			public Keyboard.Key KeyBackward { get; set; } = Keyboard.Key.S;
			public Keyboard.Key KeyLeft { get; set; } = Keyboard.Key.A;
			public Keyboard.Key KeyRight { get; set; } = Keyboard.Key.D;
			public Keyboard.Key KeyJump { get; set; } = Keyboard.Key.Space;
			public Keyboard.Key KeyDuck { get; set; } = Keyboard.Key.LControl;
			public Keyboard.Key KeyFreeMouse { get; set; } = Keyboard.Key.LAlt;
			public bool IsRotatingWithMouse { get; set; } = true;
			public bool MouseIsFree { get; set; }

			public float FieldOfView { get; set; } = 80;
			public float Sight { get; set; } = 9999;
			public int RayAmount { get; set; } = 640;
			public float RotationSpeed { get; set; } = 0.2f;
			public float MovementSpeed { get; set; } = 100;
			public float AngleHeight { get; set; }
			public float Height { get; set; }
			public float JumpStrength { get; set; } = 4f;
			public float Gravity { get; set; } = 1;
			public bool IsOnGround => Height == currentTallness - TALLNESS;

			internal void CaptureAndProcessInput()
			{
				var prevPos = Position;

				CalculateJump();
				TryRotate();
				TryMove();
				UpdateHitbox();
				CheckCollision();

				void CalculateJump()
				{
					jumpVelocity -= Time.Delta * Gravity * 10f;
					Height += jumpVelocity * Time.Delta * 60f;
					Height = MathF.Max(Height, currentTallness - TALLNESS);
				}
				void CheckCollision()
				{
					for (int i = 0; i < SpriteRaycasts.Count; i++)
					{
						var spr = SpriteRaycasts[i];
						var tallness = currentTallness - TALLNESS;
						var isAbove = spr.Height + spr.Size.Y < Height - tallness;
						var isBellow = spr.Height > Height - tallness + currentTallness;

						if (spr.IsSolid && hitbox.ConvexOverlaps(spr.Hitbox) && (isAbove == false && isBellow == false))
						{
							var crossPoints = hitbox.GetCrossPoints(spr.Hitbox);
							if (crossPoints.Count == 0)
								continue;

							var ang = crossPoints[0].AngleBetweenPoints(prevPos);
							PlayerInstance.Position = prevPos.PointMoveAtAngle(ang, MovementSpeed * 0.2f);
						}
					}
				}
				void UpdateHitbox()
				{
					hitbox.LocalLines.Clear();
					var sz = 8;
					hitbox.LocalLines.Add(new(new(-sz, -sz), new(sz, -sz)));
					hitbox.LocalLines.Add(new(new(sz, -sz), new(sz, sz)));
					hitbox.LocalLines.Add(new(new(sz, sz), new(-sz, sz)));
					hitbox.LocalLines.Add(new(new(-sz, sz), new(-sz, -sz)));
					hitbox.TransformLocalLines(this);
				}
				void TryMove()
				{
					var dir = new Vector2();
					if (Keyboard.IsKeyPressed(KeyForward)) dir += new Vector2(1, 0);
					if (Keyboard.IsKeyPressed(KeyBackward)) dir += new Vector2(-1, 0);
					if (Keyboard.IsKeyPressed(KeyLeft)) dir += new Vector2(0, -1);
					if (Keyboard.IsKeyPressed(KeyRight)) dir += new Vector2(0, 1);
					if (IsOnGround && Keyboard.IsKeyPressed(KeyJump).Once("player-jump-ffalskjf")) jumpVelocity = JumpStrength;
					if (Keyboard.IsKeyPressed(KeyDuck).Once("player-duck-ffalskjf")) jumpVelocity = IsOnGround ? 0 : jumpVelocity;
					if (IsOnGround && (Keyboard.IsKeyPressed(KeyDuck) == false).Once("player-unduck-ffalskjf")) jumpVelocity = 1;
					currentTallness = Keyboard.IsKeyPressed(KeyDuck) ? DUCK_TALLNESS : TALLNESS;

					if (dir != default)
					{
						var ms = MovementSpeed * (currentTallness != TALLNESS ? DUCK_SLOW_MULTIPLIER : 1);
						Position = Position.PointMoveAtAngle(Angle + dir.DirectionToAngle(), ms);
					}
				}
				void TryRotate()
				{
					if (IsRotatingWithMouse == false)
						return;

					if (KeyFreeMouse != Keyboard.Key.Unknown)
						MouseIsFree = Keyboard.IsKeyPressed(KeyFreeMouse);

					if (MouseIsFree)
						return;

					var anchorPoint = new Vector2(VideoMode.DesktopMode.Width * 0.5f, VideoMode.DesktopMode.Height * 0.5f);
					var mousePos = Mouse.GetPosition();

					Angle += (mousePos.X - anchorPoint.X) * RotationSpeed;
					AngleHeight += (mousePos.Y - anchorPoint.Y) * RotationSpeed * 20;
					Mouse.SetPosition(new((int)anchorPoint.X, (int)anchorPoint.Y));
				}
			}
			internal void UpdateRays()
			{
				rays.LocalLines.Clear();

				for (float i = 0; i < RayAmount; i++)
				{
					var ang = i.Map(0, RayAmount, -FieldOfView * 0.5f, FieldOfView * 0.5f);
					var line = new Line(new(), new Vector2().PointMoveAtAngle(ang, Sight, false));
					rays.LocalLines.Add(line);
				}
				rays.TransformLocalLines(this);
			}
		}
		public class SpriteRaycast : Sprite
		{
			internal Line Line
			{
				get
				{
					var p1 = Position;
					var p2 = Position.PointMoveAtAngle(Angle, Size.X * 0.3f, false);
					var resultP1 = p1.PointPercentTowardPoint(p2, -new Vector2(OriginUnit.X * 100));
					var resultP2 = p2.PointPercentTowardPoint(p1, new Vector2(OriginUnit.X * 100));
					return new(resultP1, resultP2);
				}
			}

			public static new string TexturePath { get; set; }
			[JsonIgnore]
			public static new Texture Texture => TexturePath != null && CurrentScene.Textures.ContainsKey(TexturePath) ? CurrentScene.Textures[TexturePath] : null;
			public static new BlendMode BlendMode { get; set; } = BlendMode.Alpha;
			public static float FadeDistance { get; set; } = 6;

			public new Vector2 CornerA => Line.A;
			public new Vector2 CornerB => Line.B;
			public new Vector2 CornerC => Line.A;
			public new Vector2 CornerD => Line.B;

			public bool IsSolid { get; set; } = true;
			public float Height { get; set; }
			public bool IsDrawingHeightShadow { get; set; }
			public Color HeightShadowColor { get; set; }

			public override void Draw(Camera camera = null) { }
			internal void Update()
			{
				OnUpdate();

				Hitbox.Lines.Clear();
				Hitbox.Lines.Add(Line);
			}
			internal virtual void OnUpdate() { }
		}
		public class Wall : SpriteRaycast { }
		public class Entity : SpriteRaycast
		{
			public Entity()
			{
				Size = new(50, 75);
			}
			internal override void OnUpdate()
			{
				Angle = Position.AngleBetweenPoints(PlayerInstance.Position) - 90;
			}
		}

		public FirstPersonRaycaster(string spriteRaycastTexPath, string skyTexPath = null)
		{
			SpriteRaycast.TexturePath = spriteRaycastTexPath;
			SkyTexturePath = skyTexPath;
		}

		private static float ColumnWidth => MainCamera.Resolution.X / PlayerInstance.RayAmount;

		public static string SkyTexturePath { get; set; }
		[JsonIgnore]
		public static Texture SkyTexture => SkyTexturePath != null && CurrentScene.Textures.ContainsKey(SkyTexturePath) ? CurrentScene.Textures[SkyTexturePath] : null;

		public static Color SkyColorNear { get; set; }
		public static Color SkyColorFar { get; set; }
		public static Color GroundColorNear { get; set; } = new(150, 150, 150);
		public static Color GroundColorFar { get; set; } = new(50, 50, 50);

		public static Player PlayerInstance { get; private set; }
		public static List<SpriteRaycast> SpriteRaycasts { get; } = new();

		protected override AssetQueue OnRequireAssets()
		{
			return new() { Textures = new() { SkyTexturePath, SpriteRaycast.TexturePath } };
		}
		protected override void OnStart()
		{
			PlayerInstance = new();
		}
		protected override void OnUpdate()
		{
			PlayerInstance.CaptureAndProcessInput();
			PlayerInstance.UpdateRays();

			UpdateEntities();
			DrawBackground();
			DrawSpriteRaycasts();

			for (int i = 0; i < SpriteRaycasts.Count; i++)
			{
				SpriteRaycasts[i].Hitbox.Draw();
				PlayerInstance.hitbox.Draw();
			}
		}

		private static void UpdateEntities()
		{
			for (int i = 0; i < SpriteRaycasts.Count; i++)
				SpriteRaycasts[i].Update();
		}
		private static void DrawSpriteRaycasts()
		{
			if (SpriteRaycasts == null)
				return;

			var columns = new SortedDictionary<float, List<Vertex[]>>();
			for (int j = 0; j < SpriteRaycasts.Count; j++)
			{
				var spr = SpriteRaycasts[j];
				var rays = PlayerInstance.rays.Lines;
				var w = ColumnWidth;
				var camW = MainCamera.Resolution.X * 0.5f;
				var y = -PlayerInstance.AngleHeight;

				for (int i = 0; i < rays.Count; i++)
				{
					var ray = rays[i];
					var crossPoint = ray.GetCrossPoint(spr.Line);

					if (crossPoint.IsNaN())
						continue;

					var l = i * w - camW;
					var r = (i + 1) * w - camW;
					var rayAng = ray.Angle - PlayerInstance.Angle;
					var h = new Line(ray.A, crossPoint).Length;
					var wt = SpriteRaycast.Texture == null ? 0 : SpriteRaycast.Texture.Size.X;
					var ht = SpriteRaycast.Texture == null ? 0 : SpriteRaycast.Texture.Size.Y;
					var rawLength = spr.Line.A.DistanceBetweenPoints(crossPoint);
					var texX = rawLength.Map(0, spr.Line.Length, spr.TexCoordsUnitA.X, spr.TexCoordsUnitB.X) * wt;
					var texY1 = spr.TexCoordsUnitA.Y * ht;
					var texY2 = spr.TexCoordsUnitB.Y * ht;

					h = FixFisheye(h, rayAng).Map(0, 1, 0, MainCamera.Resolution.Y) * 20;

					var top = y - h;
					var bot = y + h;
					var t = (spr.Height + spr.Size.Y - PlayerInstance.Height).Map(0, 100, bot, top);
					var b = (spr.Height - PlayerInstance.Height).Map(0, 100, bot, top);
					var shadowB = (-PlayerInstance.Height).Map(0, 100, bot, top);

					var c = (h * SpriteRaycast.FadeDistance).Limit(0, MainCamera.Resolution.Y).Map(0, MainCamera.Resolution.Y, 0, 1);
					var color = new Vector3(spr.Tint.R, spr.Tint.G, spr.Tint.B) * c;
					var tint = new Color((byte)color.X, (byte)color.Y, (byte)color.Z);
					var verts = new Vertex[4];
					var shadowVerts = new Vertex[4];

					verts[0] = new(new(l, t), tint, new(texX, texY1));
					verts[1] = new(new(r, t), tint, new(texX, texY1));
					verts[2] = new(new(r, b), tint, new(texX, texY2));
					verts[3] = new(new(l, b), tint, new(texX, texY2));

					shadowVerts[0] = new(new(l, shadowB + 1), new Color(0, 0, 0, 100));
					shadowVerts[1] = new(new(r, shadowB + 1), new Color(0, 0, 0, 100));
					shadowVerts[2] = new(new(r, shadowB - 1), new Color(0, 0, 0, 100));
					shadowVerts[3] = new(new(l, shadowB - 1), new Color(0, 0, 0, 100));

					if (columns.ContainsKey(h))
					{
						columns[h].Add(shadowVerts);
						columns[h].Add(verts);
					}
					else
						columns[h] = new() { shadowVerts, verts };
				}
			}

			var resultVerts = new VertexArray(PrimitiveType.Quads);
			foreach (var kvp in columns)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					var column = kvp.Value[i];
					resultVerts.Append(column[0]);
					resultVerts.Append(column[1]);
					resultVerts.Append(column[2]);
					resultVerts.Append(column[3]);
				}

			MainCamera.RenderTexture.Draw(resultVerts, new(SpriteRaycast.BlendMode, Transform.Identity, SpriteRaycast.Texture, null));

			float FixFisheye(float rayLength, float rayAngle) => 1 / (rayLength * MathF.Cos(rayAngle.DegreesToRadians()));
		}
		private static void DrawBackground()
		{
			var hasTexture = SkyTexture != null;
			if (hasTexture)
				SkyTexture.Repeated = true;

			var x = MainCamera.Resolution.X * 0.5f;
			var y = MainCamera.Resolution.Y * 0.5f;

			var xAng = PlayerInstance.Angle.AngleTo360() / 360;
			var yAng = PlayerInstance.AngleHeight / MainCamera.Resolution.Y;
			var topSkyL = new Vector2(xAng * 5, yAng + 0);
			var topSkyR = new Vector2(xAng * 5 + 1, yAng + 0);
			var botSkyL = new Vector2(xAng * 5, yAng + 1);
			var botSkyR = new Vector2(xAng * 5 + 1, yAng + 1);

			var vertsSky = new Vertex[]
			{
				new(new(-x, -y), Color.White, hasTexture ? new(topSkyL.X * SkyTexture.Size.X, topSkyL.Y * SkyTexture.Size.Y) : new()),
				new(new(x, -y), Color.White, hasTexture ? new(topSkyR.X * SkyTexture.Size.X, topSkyR.Y * SkyTexture.Size.Y) : new()),
				new(new(x, y), Color.White, hasTexture ? new(botSkyR.X * SkyTexture.Size.X, botSkyR.Y * SkyTexture.Size.Y) : new()),
				new(new(-x, y), Color.White, hasTexture ? new(botSkyL.X * SkyTexture.Size.X, botSkyL.Y * SkyTexture.Size.Y) : new()),
			};
			var verts = new Vertex[]
			{
				new(new(-x, -PlayerInstance.AngleHeight), GroundColorFar),
				new(new(x, -PlayerInstance.AngleHeight), GroundColorFar),
				new(new(x, y), GroundColorNear),
				new(new(-x, y), GroundColorNear),

				new(new(-x, -y), SkyColorFar),
				new(new(x, -y), SkyColorFar),
				new(new(x, -PlayerInstance.AngleHeight), SkyColorNear),
				new(new(-x, -PlayerInstance.AngleHeight), SkyColorNear),
			};
			MainCamera.RenderTexture.Draw(vertsSky, PrimitiveType.Quads, new(SkyTexture));
			MainCamera.RenderTexture.Draw(verts, PrimitiveType.Quads);
		}
	}
}
