namespace SMPL
{
	public class Thing
	{
		internal readonly static Dictionary<string, Thing> objs = new();

		private readonly List<string> childrenUIDs = new();
		private string parentUID;
		private Vector2 localPos;
		private float localAng, localSc;
		private Matrix3x2 global;
		private string uid;

		internal string UID
		{
			get => uid;
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					value = "";

				value = value.Trim();

				var i = 1;
				var freeUID = value;

				while(objs.ContainsKey(freeUID))
				{
					freeUID = $"{value}{i}";
					i++;
				}

				if(uid != null)
					objs.Remove(uid);

				uid = freeUID;
				objs[uid] = this;
			}
		}
		internal string ParentUID
		{
			get => parentUID;
			set
			{
				if(parentUID == value)
					return;

				var parent = Get(parentUID);

				if(parent != null && childrenUIDs != null)
					parent.childrenUIDs.Remove(uid);

				var prevPos = Position;
				var prevAng = Angle;
				var prevSc = Scale;

				parentUID = value;

				if(parent != null && childrenUIDs != null)
					parent.childrenUIDs.Add(uid);

				Position = prevPos;
				Angle = prevAng;
				Scale = prevSc;
			}
		}
		internal ReadOnlyCollection<string> ChildrenUIDs => childrenUIDs.AsReadOnly();

		internal Vector2 LocalDirection
		{
			get => Vector2.Normalize(LocalAngle.AngleToDirection());
			set => LocalAngle = Vector2.Normalize(value).DirectionToAngle();
		}
		internal Vector2 Direction
		{
			get => Vector2.Normalize(Angle.AngleToDirection());
			set => Angle = Vector2.Normalize(value).DirectionToAngle();
		}
		internal Vector2 LocalPosition
		{
			get => localPos;
			set { localPos = value; UpdateSelfAndChildren(); }
		}
		internal float LocalScale
		{
			get => localSc;
			set { localSc = value; UpdateSelfAndChildren(); }
		}
		internal float LocalAngle
		{
			get => localAng;
			set { localAng = value; UpdateSelfAndChildren(); }
		}

		internal Vector2 Position
		{
			get => GetPosition(global);
			set => LocalPosition = GetLocalPositionFromParent(value);
		}
		internal float Scale
		{
			get => GetScale(global);
			set => LocalScale = GetScale(GlobalToLocal(value, Angle, Position));
		}
		internal float Angle
		{
			get => GetAngle(global);
			set => LocalAngle = GetAngle(GlobalToLocal(Scale, value, Position));
		}

		internal Vector2 GetLocalPositionFromParent(Vector2 position)
		{
			return GetPosition(GlobalToLocal(Scale, Angle, position));
		}
		internal Vector2 GetPositionFromParent(Vector2 localPosition)
		{
			return GetPosition(LocalToGlobal(LocalScale, LocalAngle, localPosition));
		}
		internal Vector2 GetLocalPositionFromSelf(Vector2 position)
		{
			var m = Matrix3x2.Identity;
			m *= Matrix3x2.CreateTranslation(position);
			m *= Matrix3x2.CreateTranslation(Position);

			return GetPosition(m);
		}
		internal Vector2 GetPositionFromSelf(Vector2 localPosition)
		{
			var m = Matrix3x2.Identity;
			m *= Matrix3x2.CreateTranslation(localPosition);
			m *= Matrix3x2.CreateRotation(LocalAngle.DegreesToRadians());
			m *= Matrix3x2.CreateScale(LocalScale);
			m *= Matrix3x2.CreateTranslation(LocalPosition);

			return GetPosition(m * GetParentMatrix());
		}

		internal Thing(string uid)
		{
			UID = uid;
			LocalScale = 1;
		}

		internal virtual void OnDestroy() { }
		internal void Destroy(bool includeChildren = true)
		{
			if(includeChildren)
				for(int i = 0; i < childrenUIDs.Count; i++)
				{
					var child = Get(childrenUIDs[i]);
					child?.Destroy();
				}

			OnDestroy();
		}
		internal virtual Vector2 GetCornerClockwise(int index) => Position;

		internal Matrix3x2 LocalToGlobal(float localScale, float localAngle, Vector2 localPosition)
		{
			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(localScale);
			c *= Matrix3x2.CreateRotation(localAngle.DegreesToRadians());
			c *= Matrix3x2.CreateTranslation(localPosition);

			return c * GetParentMatrix();
		}
		internal Matrix3x2 GlobalToLocal(float scale, float angle, Vector2 position)
		{
			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(scale);
			c *= Matrix3x2.CreateRotation(angle.DegreesToRadians());
			c *= Matrix3x2.CreateTranslation(position);

			return c * GetInverseParentMatrix();
		}
		internal Matrix3x2 GetParentMatrix()
		{
			var p = Matrix3x2.Identity;
			var parent = Get(parentUID);
			if(parent != null)
			{
				p *= Matrix3x2.CreateScale(parent.Scale);
				p *= Matrix3x2.CreateRotation(parent.Angle.DegreesToRadians());
				p *= Matrix3x2.CreateTranslation(parent.Position);
			}
			return p;
		}
		internal Matrix3x2 GetInverseParentMatrix()
		{
			var inverseParent = Matrix3x2.Identity;
			var parent = Get(parentUID);
			if(parent != null)
			{
				Matrix3x2.Invert(Matrix3x2.CreateScale(parent.Scale), out var s);
				Matrix3x2.Invert(Matrix3x2.CreateRotation(parent.Angle.DegreesToRadians()), out var r);
				Matrix3x2.Invert(Matrix3x2.CreateTranslation(parent.Position), out var t);

				inverseParent *= t;
				inverseParent *= r;
				inverseParent *= s;
			}

			return inverseParent;
		}

		private void UpdateSelfAndChildren()
		{
			UpdateGlobalMatrix();

			for(int i = 0; i < childrenUIDs.Count; i++)
			{
				var child = Get(childrenUIDs[i]);
				child?.UpdateSelfAndChildren();
			}
		}
		private void UpdateGlobalMatrix()
		{
			global = LocalToGlobal(LocalScale, LocalAngle, LocalPosition);
		}

		internal static float GetAngle(Matrix3x2 matrix)
		{
			return MathF.Atan2(matrix.M12, matrix.M11).RadiansToDegrees();
		}
		internal static Vector2 GetPosition(Matrix3x2 matrix)
		{
			return new(matrix.M31, matrix.M32);
		}
		internal static float GetScale(Matrix3x2 matrix)
		{
			return MathF.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
		}

		internal static Thing Get(string uid, int depth = 1)
		{
			return Get<Thing>(uid, depth);
		}
		internal static T Get<T>(string uid, int depth = 1) where T : Thing
		{
			if(uid == null || objs.ContainsKey(uid) == false)
				return default;

			if(objs[uid] is not T)
			{
				Console.LogError(depth + 1, $"The {nameof(Thing)} with UID '{uid}' exists but is not a {typeof(T).Name} - it is a {objs[uid].GetType().Name}");
				return default;
			}

			return (T)objs[uid];
		}
	}
}
