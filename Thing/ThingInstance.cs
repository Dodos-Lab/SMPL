namespace SMPL
{
	internal abstract class ThingInstance
	{
		public ReadOnlyCollection<string> Types
		{
			get
			{
				var result = new List<string>();
				var curType = GetType();

				Add(curType);
				while(curType.BaseType != null)
				{
					Add(curType.BaseType);
					curType = curType.BaseType;
				}
				return result.AsReadOnly();

				void Add(Type type)
				{
					if(type.Name == nameof(Object))
						return;

					result.Add(type.Name.Replace("Instance", ""));
				}
			}
		}
		public string UID
		{
			get => uid;
			set
			{
				if(value == uid)
					return;

				var objs = Scene.CurrentScene.objs;

				if(uid != null)
					objs.Remove(uid);

				oldUID = uid;
				uid = value;
				objs[uid] = this;

				var parent = Get(parentUID);
				if(parent != null)
				{
					parent.childrenUIDs.Remove(oldUID);
					parent.childrenUIDs.Add(uid);
				}

				for(int i = 0; i < childrenUIDs.Count; i++)
				{
					var child = Get(childrenUIDs[i]);
					if(child != null)
						child.parentUID = uid;
				}
			}
		}
		public string OldUID => oldUID;

		public List<string> Tags { get; } = new();

		[JsonIgnore]
		public float Age => age.ElapsedTime.AsSeconds();

		public string ParentUID
		{
			get => parentUID;
			set
			{
				if(parentUID == value || uid == value)
					return;

				if(childrenUIDs.Contains(value))
				{
					var child = Get(value);
					child.ParentUID = null;
				}

				var parent = Get(parentUID);

				if(parent != null)
					parent.childrenUIDs.Remove(uid);

				var prevPos = Position;
				var prevAng = Angle;
				var prevSc = Scale;

				parOldUID = parentUID;
				parentUID = value;

				Position = prevPos;
				Angle = prevAng;
				Scale = prevSc;

				var newParent = Get(parentUID);
				if(newParent != null && newParent.childrenUIDs.Contains(uid) == false)
					newParent.childrenUIDs.Add(uid);
			}
		}
		public string ParentOldUID => parOldUID;
		[JsonIgnore]
		public ReadOnlyCollection<string> ChildrenUIDs => childrenUIDs.AsReadOnly();

		public Vector2 LocalPosition
		{
			get => localPos;
			set { localPos = value; UpdateSelfAndChildren(); }
		}
		public float LocalScale
		{
			get => localSc;
			set { localSc = value; UpdateSelfAndChildren(); }
		}
		public float LocalAngle
		{
			get => localAng.AngleTo360();
			set { localAng = value; UpdateSelfAndChildren(); }
		}
		public Vector2 LocalDirection
		{
			get => Vector2.Normalize(LocalAngle.AngleToDirection());
			set => LocalAngle = Vector2.Normalize(value).DirectionToAngle();
		}

		[JsonIgnore]
		public Vector2 Position
		{
			get => GetPosition(global);
			set => LocalPosition = GetLocalPositionFromParent(value);
		}
		[JsonIgnore]
		public float Scale
		{
			get => GetScale(global);
			set => LocalScale = GetScale(GlobalToLocal(value, Angle, Position));
		}
		[JsonIgnore]
		public float Angle
		{
			get => GetAngle(global).AngleTo360();
			set => LocalAngle = GetAngle(GlobalToLocal(Scale, value, Position));
		}
		[JsonIgnore]
		public Vector2 Direction
		{
			get => Vector2.Normalize(Angle.AngleToDirection());
			set => Angle = Vector2.Normalize(value).DirectionToAngle();
		}

		public Hitbox Hitbox
		{
			get
			{
				hitbox.TransformLocalLines(uid);
				return hitbox;
			}
		}
		[JsonIgnore]
		public Hitbox BoundingBox
		{
			get
			{
				var bb = GetBoundingBox();
				bb.TransformLocalLines(uid);
				return bb;
			}
		}

		public Vector2 GetLocalPositionFromParent(Vector2 position)
		{
			return GetPosition(GlobalToLocal(Scale, Angle, position));
		}
		public Vector2 GetPositionFromParent(Vector2 localPosition)
		{
			return GetPosition(LocalToGlobal(LocalScale, LocalAngle, localPosition));
		}
		public Vector2 GetLocalPositionFromSelf(Vector2 position)
		{
			Matrix3x2.Invert(global, out var m);
			return Vector2.Transform(position, m);
		}
		public Vector2 GetPositionFromSelf(Vector2 localPosition)
		{
			return Vector2.Transform(localPosition, global);
		}

		public void Destroy(bool includeChildren)
		{
			OnDestroy();

			var children = new List<string>(childrenUIDs);
			for(int i = 0; i < children.Count; i++)
			{
				var child = Get(children[i]);
				if(child == null)
					continue;

				// prevents hard jump to world coordinates
				child.ParentUID = null;

				if(includeChildren)
					child.Destroy(includeChildren);
			}
			Scene.CurrentScene.objs.Remove(uid);

			// prevents hard jump to world coordinates
			for(int i = 0; i < children.Count; i++)
			{
				var child = Get(children[i]);
				if(child == null)
					continue;

				child.ParentUID = uid;
			}
		}
		public override string ToString()
		{
			return $"{GetType().GetPrettyName()}{{{uid}}}";
		}

		#region Backend
		private const float DEFAULT_BB_SIZE = 50f;
		private readonly Clock age = new();
		private readonly Hitbox hitbox = new();
		private readonly List<string> childrenUIDs = new();
		private string parentUID, oldUID, parOldUID;
		private Vector2 localPos;
		private float localAng, localSc;
		private Matrix3x2 global;
		private string uid;

		[JsonConstructor]
		internal ThingInstance() { }
		internal ThingInstance(string uid)
		{
			UID = Thing.GetFreeUID(uid);
			LocalScale = 1;

			// i just got born, do i have any children before i was even born? claim ownership if so
			// or perhaps i am a child of theirs before i was even born? approve their parentship if so
			// (the longer living object wins parentship if both cases are true for some reason)
			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
			{
				if(kvp.Value.parentUID == uid)
					childrenUIDs.Add(kvp.Key);
				else if(kvp.Value.childrenUIDs.Contains(uid))
					ParentUID = kvp.Value.UID;
			}
		}

		internal virtual void OnDestroy() { }
		internal virtual Hitbox GetBoundingBox()
		{
			var sz = DEFAULT_BB_SIZE;
			return new Hitbox(
				new(-sz, -sz),
				new(sz, -sz),
				new(sz, sz),
				new(-sz, sz),
				new(-sz, -sz));
		}

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

		internal static ThingInstance Get(string uid)
		{
			return Get<ThingInstance>(uid);
		}
		internal static T Get<T>(string uid) where T : ThingInstance
		{
			var objs = Scene.CurrentScene.objs;
			return Thing.Exists(uid) == false || objs[uid] is not T ? default : (T)objs[uid];
		}
		internal static ThingInstance GetTryError(string uid)
		{
			var obj = Get(uid);
			if(obj == null)
			{
				MissingError(uid);
				return default;
			}
			return obj;
		}
		internal static void MissingError(string uid)
		{
			Console.LogError(0, $"Thing{{{uid}}} does not exist.");
		}
		#endregion
	}
}
