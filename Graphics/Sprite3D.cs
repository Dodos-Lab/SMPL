namespace SMPL.Graphics
{
	internal class Sprite3D : Sprite
	{
		internal float Height { get; set; } = 20;
		internal float Angle3D { get; set; } = 270;

		internal Sprite3D(string uid, Scene.TexturedModel3D texturedModel3D) : base(uid)
		{
			if(texturedModel3D.ObjModelPath == null || texturedModel3D.TexturePath == null ||
				texturedModel3D.TextureCount == 0 || File.Exists(texturedModel3D.ObjModelPath) == false || texturedModel3D.TextureDetail <= 0)
				throw new System.Exception();

			var img = default(Image);
			if(File.Exists(texturedModel3D.TexturePath))
				img = new Image(texturedModel3D.TexturePath);
			var content = File.ReadAllText(texturedModel3D.ObjModelPath).Replace('\r', ' ').Split('\n');
			var layeredImages = new List<Image>();
			var indexTexCoords = new List<int>();
			var indexVert = new List<int>();
			var texCoords = new List<Vector3>();
			var verts = new List<Vector3>();
			var boundingBoxA = new Vector3();
			var boundingBoxB = new Vector3();

			for(int i = 0; i < content.Length; i++)
			{
				var split = content[i].Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
				if(split.Length == 0)
					continue;

				switch(split[0])
				{
					case "v":
						{
							var v = new Vector3(N(1), N(2), N(3)) * texturedModel3D.Scale;
							verts.Add(v);

							if(v.X < boundingBoxA.X)
								boundingBoxA.X = v.X;
							if(v.Y < boundingBoxA.Y)
								boundingBoxA.Y = v.Y;
							if(v.Z < boundingBoxA.Z)
								boundingBoxA.Z = v.Z;

							if(v.X > boundingBoxB.X)
								boundingBoxB.X = v.X;
							if(v.Y > boundingBoxB.Y)
								boundingBoxB.Y = v.Y;
							if(v.Z > boundingBoxB.Z)
								boundingBoxB.Z = v.Z;

							break;
						}
					case "vt": texCoords.Add(new(N(1), 1 - N(2), 1)); break;
					case "f":
						{
							for(int j = 1; j < split.Length; j++)
							{
								var face = split[j].Split('/');
								indexVert.Add(int.Parse(face[0]) - 1);

								if(face.Length > 1 && face[1].Length != 0)
									indexTexCoords.Add(int.Parse(face[1]) - 1);
							}
							break;
						}
				}
				float N(int i) => float.Parse(split[i]);
			}

			var vertsOffset = boundingBoxA;
			var detail = texturedModel3D.TextureDetail;
			boundingBoxB -= vertsOffset;
			var textureSize = boundingBoxB * detail;

			for(int i = 0; i < texturedModel3D.TextureCount; i++)
				layeredImages.Add(new Image((uint)textureSize.X, (uint)textureSize.Z, Color.Transparent));

			for(int i = 0; i < indexVert.Count; i++)
			{
				var p = verts[indexVert[i]];
				var tx = i < indexTexCoords.Count ? texCoords[indexTexCoords[i]] : default;
				var resultTexCoordsX = (uint)(p.X - vertsOffset.X).Map(0, boundingBoxB.X, 0, (uint)textureSize.X - 1);
				var resultTexCoordsY = (uint)(p.Z - vertsOffset.Z).Map(0, boundingBoxB.Z, 0, (uint)textureSize.Z - 1);
				var textureIndex = (int)(p.Y - vertsOffset.Y).Map(0, boundingBoxB.Y, 0, texturedModel3D.TextureCount - 1);
				var color = img == null ? Color.White : img.GetPixel((uint)(tx.X * img.Size.X), (uint)(tx.Y * img.Size.Y));

				layeredImages[textureIndex].SetPixel(resultTexCoordsX, resultTexCoordsY, color);
			}

			textureCount = layeredImages.Count;
			texturePaths = new string[textureCount];
			for(int i = 0; i < layeredImages.Count; i++)
			{
				var id = GetTextureID(i);
				Scene.CurrentScene.Textures[id] = new Texture(layeredImages[i]);
				texturePaths[i] = id;
				layeredImages[i].Dispose();
			}
			img?.Dispose();
		}
		internal Sprite3D(string uid, ICollection<string> texturePaths) : base(uid) => this.texturePaths = texturePaths.ToArray();

		internal void SetHitbox3D()
		{
			var points = new List<Vector2>();

			for(int i = 0; i < 4; i++)
				points.Add(CornerClockwise(i));
			for(int i = 0; i < 4; i++)
				points.Add(Corner3DClockwise(i));

			var outline = points.OutlinePoints();

			Hitbox.Lines.Clear();
			for(int i = 1; i < outline.Count; i++)
				Hitbox.Lines.Add(new(outline[i - 1], outline[i]));
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden || textureCount == 0)
				return;

			var camera = Get<Camera>(CameraUID);
			if(camera != null)
				renderTarget = camera.renderTexture;
			var h = Height * Scale / textureCount;
			for(int i = 0; i < textureCount; i++)
			{
				var tex = Scene.CurrentScene.Textures[texturePaths[i]];
				var verts = new Vertex[]
				{
					new(CornerClockwise(0).PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(0, 0)),
					new(CornerClockwise(1).PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(tex.Size.X, 0)),
					new(CornerClockwise(2).PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(tex.Size.X, tex.Size.Y)),
					new(CornerClockwise(3).PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(0, tex.Size.Y)),
				};

				renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader()));
			}
		}
		internal override Hitbox GetBoundingBox()
		{
			return base.GetBoundingBox();
		}

		internal Vector2 Corner3DClockwise(int index)
		{
			index = index.Limit(0, 4, Extensions.Limitation.Overflow);

			var h = Height * (textureCount == 0 ? 1 : textureCount);
			return index switch
			{
				0 => CornerClockwise(0).PointMoveAtAngle(Angle3D, h, false),
				1 => CornerClockwise(1).PointMoveAtAngle(Angle3D, h, false),
				2 => CornerClockwise(2).PointMoveAtAngle(Angle3D, h, false),
				3 => CornerClockwise(3).PointMoveAtAngle(Angle3D, h, false),
				_ => default,
			};
		}

		#region Backend
		private readonly string[] texturePaths;
		private readonly int textureCount;

		private string GetTextureID(int i) => $"sprite3d-texture-{i}-{GetHashCode()}";
		#endregion
	}
}
