using SFML.Graphics;
using SMPL.Core;
using SMPL.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SMPL.Graphics
{
	/// <summary>
	/// Inherit chain: <see cref="Sprite3D"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
	/// A "3D" <see cref="Sprite"/> that fakes depth by stacking a set amount of textures on top of each other. Also known as
	/// the 'sprite stacking technique'.
	/// </summary>
	public class Sprite3D : Sprite
   {
		private readonly Texture[] textures;

		/// <summary>
		/// The amount of faked depth that the spacing between all textures sum up to.
		/// </summary>
		public float Height { get; set; } = 20;
		/// <summary>
		/// The angle at which the textures are stacked toward. Or in other words: the orthographic up angle of the <see cref="Sprite3D"/>.
		/// </summary>
		public float Angle3D { get; set; } = 270;

		/// <summary>
		/// Initially this is the top left corner of the very top texture of this <see cref="Sprite3D"/> in the world.
		/// </summary>
		public Vector2 Corner3DA => CornerA.PointMoveAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);
		/// <summary>
		/// Initially this is the top right corner of the very top texture of this <see cref="Sprite3D"/> in the world.
		/// </summary>
		public Vector2 Corner3DB => CornerB.PointMoveAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);
		/// <summary>
		/// Initially this is the bottom right corner of the very top texture of this <see cref="Sprite3D"/> in the world.
		/// </summary>
		public Vector2 Corner3DC => CornerC.PointMoveAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);
		/// <summary>
		/// Initially this is the bottom left corner of the very top texture of this <see cref="Sprite3D"/> in the world.
		/// </summary>
		public Vector2 Corner3DD => CornerD.PointMoveAtAngle(Angle3D, Height * (textures?.Length ?? 1), false);

		/// <summary>
		/// Construct the <see cref="Sprite3D"/> from a 3D model .obj file and its texture with additional details.
		/// </summary>
		public Sprite3D(Scene.TexturedModel3D texturedModel3D)
		{
			if (texturedModel3D.ObjModelPath == null || texturedModel3D.TexturePath == null ||
				texturedModel3D.TextureCount == 0 || File.Exists(texturedModel3D.ObjModelPath) == false || texturedModel3D.TextureDetail <= 0)
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
		/// <summary>
		/// Construct the <see cref="Sprite3D"/> from some <paramref name="textures"/>.
		/// </summary>
      public Sprite3D(params Texture[] textures) => this.textures = textures;
		/// <summary>
		/// Construct the <see cref="Sprite3D"/> from some <paramref name="textures"/>.
		/// </summary>
		public Sprite3D(ICollection<Texture> textures) => this.textures = textures.ToArray();

		/// <summary>
		/// Set the <see cref="Sprite.Hitbox"/> to be the outline of all the textures in this <see cref="Sprite3D"/>.
		/// This needs to be updated frequently in order to maintain the correct shape (in <see cref="Scene.OnUpdate"/> for example).
		/// </summary>
		public void SetHitbox3D()
      {
			var points = new List<Vector2>() { CornerA, CornerB, CornerD, CornerC, Corner3DA, Corner3DB, Corner3DD, Corner3DC };
			var outline = points.OutlinePoints();

			Hitbox.Lines.Clear();
			for (int i = 1; i < outline.Count; i++)
				Hitbox.Lines.Add(new(outline[i - 1], outline[i]));
		}
		/// <summary>
		/// Draws all the <see cref="Sprite3D"/>'s textures on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/> and <see cref="Sprite3D"/> parameters.
		/// </summary>
		public override void Draw()
      {
			if (IsHidden || textures == null || textures.Length == 0)
				return;

			DrawTarget ??= Scene.MainCamera;

			var h = Height * Scale / textures.Length;
         for (int i = 0; i < textures.Length; i++)
         {
				Texture = textures[i];
				var verts = new Vertex[]
				{
					new(CornerA.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Color, new(0, 0)),
					new(CornerB.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Color, new(Texture.Size.X, 0)),
					new(CornerC.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Color, new(Texture.Size.X, Texture.Size.Y)),
					new(CornerD.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Color, new(0, Texture.Size.Y)),
				};

				DrawTarget.renderTexture.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
			}
		}
   }
}
