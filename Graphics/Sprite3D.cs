using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SMPL
{
   public class Sprite3D : Sprite
   {
		private readonly Texture[] textures;

		public float Height { get; set; } = 4;
		public float Angle3D { get; set; } = 270;

		public Vector2 TopLeft3D => TopLeft.MovePointAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);
		public Vector2 TopRight3D => TopRight.MovePointAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);
		public Vector2 BottomLeft3D => BottomLeft.MovePointAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);
		public Vector2 BottomRight3D => BottomRight.MovePointAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);

		public Sprite3D(Scene.TexturedModel3D texturedModel3D)
		{
			if (texturedModel3D.ObjModelPath == null || texturedModel3D.TexturePath == null ||
				texturedModel3D.TextureCount == 0 || File.Exists(texturedModel3D.ObjModelPath) == false)
				throw new System.Exception();

			var img = default(Image);
         if (File.Exists(texturedModel3D.TexturePath))
				img = new Image(texturedModel3D.TexturePath);
			var content = File.ReadAllText(texturedModel3D.ObjModelPath).Replace('\r', ' ').Split('\n');
			var layeredImages = new List<Image>();
			var indexTexCoords = new List<int>();
			var indexVert = new List<int>();
			var texCoords = new List<Vector3>();
			var verts = new List<Vector3>();
			var boundingBoxA = new Vector3();
			var boundingBoxB = new Vector3();

			for (int i = 0; i < content.Length; i++)
			{
				var split = content[i].Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
				if (split.Length == 0)
					continue;

				switch (split[0])
				{
					case "v":
						{
							var v = new Vector3(N(1), N(2), N(3)) * texturedModel3D.Scale;
							verts.Add(v);

							if (v.X < boundingBoxA.X)
								boundingBoxA.X = v.X;
							if (v.Y < boundingBoxA.Y)
								boundingBoxA.Y = v.Y;
							if (v.Z < boundingBoxA.Z)
								boundingBoxA.Z = v.Z;

							if (v.X > boundingBoxB.X)
								boundingBoxB.X = v.X;
							if (v.Y > boundingBoxB.Y)
								boundingBoxB.Y = v.Y;
							if (v.Z > boundingBoxB.Z)
								boundingBoxB.Z = v.Z;

							break;
						}
					case "vt": texCoords.Add(new(N(1), 1 - N(2), 1)); break;
					case "f":
						{
							for (int j = 1; j < split.Length; j++)
							{
								var face = split[j].Split('/');
								indexVert.Add(int.Parse(face[0]) - 1);

								if (face.Length > 1 && face[1].Length != 0)
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

			for (int i = 0; i < texturedModel3D.TextureCount; i++)
				layeredImages.Add(new Image((uint)textureSize.X, (uint)textureSize.Z, Color.Transparent));

			for (int i = 0; i < indexVert.Count; i++)
			{
				var p = verts[indexVert[i]];
				var tx = i < indexTexCoords.Count ? texCoords[indexTexCoords[i]] : default;
				var resultTexCoordsX = (uint)(p.X - vertsOffset.X).Map(0, boundingBoxB.X, 0, (uint)textureSize.X - 1);
				var resultTexCoordsY = (uint)(p.Z - vertsOffset.Z).Map(0, boundingBoxB.Z, 0, (uint)textureSize.Z - 1);
				var textureIndex = (int)(p.Y - vertsOffset.Y).Map(0, boundingBoxB.Y, 0, texturedModel3D.TextureCount - 1);
				var color = img == null ? Color.White : img.GetPixel((uint)(tx.X * img.Size.X), (uint)(tx.Y * img.Size.Y));

				layeredImages[textureIndex].SetPixel(resultTexCoordsX, resultTexCoordsY, color);
			}

			var layeredTextures = new Texture[layeredImages.Count];
			for (int i = 0; i < layeredImages.Count; i++)
			{
				layeredTextures[i] = new Texture(layeredImages[i]);
				layeredImages[i].Dispose();
			}
			img?.Dispose();

			textures = layeredTextures;
		}
      public Sprite3D(params Texture[] textures)
      {
			this.textures = textures;
      }

		public void SetHitbox3D()
      {
			var points = new List<Vector2>() { TopLeft, TopRight, BottomLeft, BottomRight, TopLeft3D, TopRight3D, BottomLeft3D, BottomRight3D };
			var outline = points.OutlinePoints();

			Hitbox.Lines.Clear();
			for (int i = 1; i < outline.Count; i++)
				Hitbox.Lines.Add(new(outline[i - 1], outline[i]));
		}
      public override void Draw()
      {
			if (IsHidden || textures == null || textures.Length == 0)
				return;

			RenderTarget ??= Game.Window;

         for (int i = 0; i < textures.Length; i++)
         {
				Texture = textures[i];
				var verts = new Vertex[]
				{
					new(TopLeft.MovePointAtAngle(Angle3D, i * Height * Scale, false).ToSFML(), Color, new(0, 0)),
					new(TopRight.MovePointAtAngle(Angle3D, i * Height * Scale, false).ToSFML(), Color, new(Texture.Size.X, 0)),
					new(BottomRight.MovePointAtAngle(Angle3D, i * Height * Scale, false).ToSFML(), Color, new(Texture.Size.X, Texture.Size.Y)),
					new(BottomLeft.MovePointAtAngle(Angle3D, i * Height * Scale, false).ToSFML(), Color, new(0, Texture.Size.Y)),
				};

				RenderTarget.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
			}
		}
   }
}
