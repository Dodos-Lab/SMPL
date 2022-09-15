namespace SMPL.Tools
{
	/// <summary>
	/// A <see cref="Line"/> collection used to determine whether it interacts in any way with other hitboxes/points in the world.
	/// </summary>
	public class Hitbox
	{
		/// <summary>
		/// This list is used by <see cref="UpdateLines(ThingInstance)"/> which transforms and moves its contents into <see cref="Lines"/>.
		/// </summary>
		public List<Line> LocalLines { get; } = new();
		/// <summary>
		/// This list is used by <see cref="UpdateLines(ThingInstance)"/> for writing and all the rest of the methods for reading.
		/// </summary>
		public List<Line> Lines { get; } = new();

		/// <summary>
		/// Returns whether this <see cref="Hitbox"/> contains the mouse cursor in the <see cref="Scene.CurrentScene"/>.
		/// This is a shortcut for <see cref="Contains"/> -> <see cref="Scene.MousePosition"/>.
		/// </summary>
		[JsonIgnore]
		public bool IsHovered => Contains(Scene.MousePosition);

		/// <summary>
		/// Constructs both <see cref="LocalLines"/> and <see cref="Lines"/> from between the <paramref name="points"/>.
		/// </summary>
		public Hitbox(params Vector2[] points)
		{
			for(int i = 1; i < points?.Length; i++)
			{
				LocalLines.Add(new(points[i - 1], points[i]));
				Lines.Add(new(points[i - 1], points[i]));
			}
		}

		/// <summary>
		/// Takes <see cref="LocalLines"/>, applies <paramref name="thingUID"/>'s transformations on them and puts the result into <see cref="Lines"/>
		/// for the rest of the methods to use. Any previous changes to the <see cref="Lines"/> list will be erased.
		/// </summary>
		public void TransformLocalLines(string thingUID)
		{
			var thing = ThingInstance.Get_(thingUID);
			if(thing == null)
				return;

			Lines.Clear();

			for(int i = 0; i < LocalLines.Count; i++)
			{
				var a = thing.PositionFromSelf(LocalLines[i].A);
				var b = thing.PositionFromSelf(LocalLines[i].B);
				Lines.Add(new(a, b));
			}
		}
		/// <summary>
		/// Draws all <see cref="Lines"/>. See <see cref="Extensions.Draw"/> for more info.
		/// </summary>
		public void Draw(RenderTarget renderTarget = default, Color color = default, float width = 4)
		{
			Lines.Draw(renderTarget, color, width);
		}

		/// <summary>
		/// Calculates and then returns all the cross points (if any) produced between <see cref="Lines"/> and <paramref name="hitbox"/>'s <see cref="Lines"/>.
		/// </summary>
		public List<Vector2> CrossPoints(Hitbox hitbox)
		{
			var result = new List<Vector2>();
			for(int i = 0; i < Lines.Count; i++)
				for(int j = 0; j < hitbox.Lines.Count; j++)
				{
					var p = Lines[i].CrossPoint(hitbox.Lines[j]);
					if(p.IsNaN() == false)
						result.Add(p);
				}
			return result;
		}
		/// <summary>
		/// A shortcut for
		/// <code>var overlaps = Crosses(hitbox) || Contains(hitbox) || hitbox.Contains(this);</code>
		/// </summary>
		public bool Overlaps(Hitbox hitbox)
		{
			return Crosses(hitbox) || Contains(hitbox) || hitbox.Contains(this);
		}
		/// <summary>
		/// Whether <see cref="Lines"/> surround <paramref name="point"/>.
		/// Or in other words: whether this <see cref="Hitbox"/> contains <paramref name="point"/>.
		/// </summary>
		public bool Contains(Vector2 point)
		{
			if(Lines == null || Lines.Count < 3)
				return false;

			var crosses = 0;
			var outsidePoint = Lines[0].A.PercentToTarget(Lines[0].B, new(-500, -500));

			for(int i = 0; i < Lines.Count; i++)
				if(Lines[i].Crosses(new(point, outsidePoint)))
					crosses++;

			return crosses % 2 == 1;
		}
		/// <summary>
		/// Whether <see cref="Lines"/> cross with <paramref name="hitbox"/>'s <see cref="Lines"/>.
		/// </summary>
		public bool Crosses(Hitbox hitbox)
		{
			for(int i = 0; i < Lines.Count; i++)
				for(int j = 0; j < hitbox.Lines.Count; j++)
					if(Lines[i].Crosses(hitbox.Lines[j]))
						return true;

			return false;
		}
		/// <summary>
		/// Whether <see cref="Lines"/> completely surround <paramref name="hitbox"/>'s <see cref="Lines"/>.
		/// Or in other words: whether this <see cref="Hitbox"/> contains <paramref name="hitbox"/>.
		/// </summary>
		public bool Contains(Hitbox hitbox)
		{
			for(int i = 0; i < hitbox.Lines.Count; i++)
				for(int j = 0; j < Lines.Count; j++)
					if((Contains(hitbox.Lines[i].A) == false || Contains(hitbox.Lines[i].B) == false))
						return false;
			return true;
		}
		/// <summary>
		/// Returns the closest point on the <see cref="Hitbox.Lines"/> to a certain <paramref name="point"/>.
		/// </summary>
		public Vector2 ClosestPoint(Vector2 point)
		{
			var points = new List<Vector2>();
			var result = new Vector2();
			var bestDist = float.MaxValue;

			for(int i = 0; i < Lines.Count; i++)
				points.Add(Lines[i].ClosestPoint(point));
			for(int i = 0; i < points.Count; i++)
			{
				var dist = points[i].Distance(point);
				if(dist < bestDist)
				{
					bestDist = dist;
					result = points[i];
				}
			}
			return result;
		}

		#region Backend
		internal struct ButtonResult
		{
			public Vector2 DragDelta { get; set; }
			public bool IsHeld { get; set; }
			public bool IsPressed { get; set; }
			public bool IsReleased { get; set; }
			public bool IsHovered { get; set; }
			public bool IsUnhovered { get; set; }
			public bool IsDragged { get; set; }
			public bool IsClicked { get; set; }
			public bool IsDropped { get; set; }
		}

		private float holdDelayTimer, holdTriggerTimer;
		private bool isClicked, preventClick;

		internal ButtonResult TryButton(float holdDelay = 0.5f, float holdTriggerSpeed = 0.1f, bool isDraggable = false, bool isHoldable = true)
		{
			var result = new ButtonResult();
			holdDelayTimer -= Time.Delta;
			holdTriggerTimer = holdTriggerTimer < 0 ? holdTriggerSpeed : holdTriggerTimer - Time.Delta;

			var hovered = IsHovered;
			var leftClicked = Mouse.IsButtonPressed(Mouse.Button.Left);
			var id = GetHashCode();
			var isTime = holdTriggerTimer < 0 && holdDelayTimer < 0;

			if(isHoldable && isTime && hovered && isClicked)
				result.IsHeld = true;

			if(hovered.Once($"{id}-hovered"))
			{
				if(isClicked)
					result.IsPressed = true;

				result.IsHovered = true;
			}

			if((hovered == false).Once($"{id}-unhovered"))
				result.IsUnhovered = true;

			if(leftClicked.Once($"{id}-press") && hovered)
			{
				isClicked = true;
				holdDelayTimer = holdDelay;
				result.IsPressed = true;

				if(isDraggable)
					result.IsDragged = true;
			}

			if((leftClicked == false).Once($"{id}-release"))
			{
				if(hovered)
				{
					if(isClicked && preventClick == false)
						result.IsClicked = true;

					if(isClicked && isDraggable)
						result.IsDropped = true;

					result.IsReleased = true;
					result.IsHovered = true;
				}
				isClicked = false;
				preventClick = false;
			}

			if(isClicked && isDraggable)
				result.DragDelta = Scene.MouseDelta;

			if(result.IsHeld)
				preventClick = true;

			return result;
		}
		#endregion
	}
}
