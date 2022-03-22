using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace SMPL
{
	public class Object
	{
		private readonly List<Object> children = new();
		private Object parent;
		private Vector2 localPos, localSc;
		private float localAng;
		private Matrix3x2 global;

		public Vector2 LocalDirection
		{
			get => Vector2.Normalize(LocalAngle.AngleToDirection());
			set => LocalAngle = Vector2.Normalize(value).DirectionToAngle();
		}
		public Vector2 Direction
		{
			get => Vector2.Normalize(Angle.AngleToDirection());
			set {  }
		}

		public Vector2 LocalPosition { get => localPos; set { localPos = value; UpdateSelfAndChildren(); } }
		public Vector2 LocalScale { get => localSc; set { localSc = value; UpdateSelfAndChildren(); } }
		public float LocalAngle { get => localAng; set { localAng = value; UpdateSelfAndChildren(); } }

		public Vector2 Position
		{
			get => GetPosition(global);
			set
			{
				var c = Matrix3x2.Identity;
				c *= Matrix3x2.CreateScale(LocalScale);
				c *= Matrix3x2.CreateRotation(LocalAngle.DegreesToRadians());
				c *= Matrix3x2.CreateTranslation(value);

				var local = c * GetInverseParent();
				LocalPosition = GetPosition(local);
			}
		}
		public Vector2 Scale
		{
			get => GetScale(global);
			set
			{
				var c = Matrix3x2.Identity;
				c *= Matrix3x2.CreateScale(value);
				c *= Matrix3x2.CreateRotation(LocalAngle.DegreesToRadians());
				c *= Matrix3x2.CreateTranslation(LocalPosition);

				var local = c * GetInverseParent();
				LocalScale = GetScale(local);
			}
		}
		public float Angle
		{
			get => GetAngle(global);
			set
			{
				var c = Matrix3x2.Identity;
				c *= Matrix3x2.CreateScale(LocalScale);
				c *= Matrix3x2.CreateRotation(value.DegreesToRadians());
				c *= Matrix3x2.CreateTranslation(LocalPosition);

				var local = c * GetInverseParent();
				LocalAngle = GetAngle(local);
			}
		}

		public Object Parent
		{
			get => parent;
			set
			{
				if (parent != value && parent != null)
					parent.children.Remove(this);

				var prevPos = Position;
				var prevAng = Angle;
				var prevSc = Scale;

				parent = value;

				if (parent != null)
					parent.children.Add(this);

				Position = prevPos;
				Angle = prevAng;
				Scale = prevSc;
			}
		}
		public ReadOnlyCollection<Object> Children => children.AsReadOnly();

		public Object()
		{
			localSc = new(1, 1);
		}

		private void UpdateSelfAndChildren()
		{
			UpdateGlobalMatrix();

			for (int i = 0; i < children.Count; i++)
				children[i].UpdateGlobalMatrix();
		}
		private void UpdateGlobalMatrix()
		{
			var p = Matrix3x2.Identity;

			if (parent != null)
			{
				p *= Matrix3x2.CreateScale(parent.Scale);
				p *= Matrix3x2.CreateRotation(parent.Angle.DegreesToRadians());
				p *= Matrix3x2.CreateTranslation(parent.Position);
			}

			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(LocalScale);
			c *= Matrix3x2.CreateRotation(LocalAngle.DegreesToRadians());
			c *= Matrix3x2.CreateTranslation(LocalPosition);

			global = c * p;
		}
		private Matrix3x2 GetInverseParent()
		{
			var p = Matrix3x2.Identity;

			if (parent != null)
			{
				Matrix3x2.Invert(Matrix3x2.CreateScale(parent.Scale), out var s);
				Matrix3x2.Invert(Matrix3x2.CreateRotation(parent.Angle.DegreesToRadians()), out var r);
				Matrix3x2.Invert(Matrix3x2.CreateTranslation(parent.Position), out var t);

				p *= t;
				p *= r;
				p *= s;
			}
			return p;
		}

		private static float GetAngle(Matrix3x2 matrix)
		{
			return MathF.Atan2(matrix.M12, matrix.M11).RadiansToDegrees();
		}
		private static Vector2 GetPosition(Matrix3x2 matrix)
		{
			return new(matrix.M31, matrix.M32);
		}
		private static Vector2 GetScale(Matrix3x2 matrix)
		{
			return new(MathF.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12),
				MathF.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22));
		}
	}
}
