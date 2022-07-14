namespace SMPL.UI
{
	internal class TextButtonInstance : ButtonInstance
	{
		public bool IsHyperlink { get; set; }
		public string TextUID
		{
			get => textUID;
			set
			{
				textUID = value;

				var text = Get<TextInstance>(TextUID);
				if(text != null)
					text.ParentUID = UID;
			}
		}

		#region Backend
		private string textUID;

		[JsonConstructor]
		internal TextButtonInstance()
		{
			Init(null);
		}
		internal TextButtonInstance(string uid, string textUID) : base(uid)
		{
			Init(textUID);
		}
		private void Init(string textUID)
		{
			Size = new(250, 60);
			TextUID = textUID;
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			var text = Get<TextInstance>(TextUID);

			if(text != null)
			{
				text.ParentUID = UID;
				text.UpdateGlobalText();
			}

			if(IsHyperlink)
			{
				text.UpdateGlobalText();
				var b = TextInstance.textInstance.GetLocalBounds();
				Size = new(b.Width * Scale, b.Height * Scale);
			}
			base.OnDraw(renderTarget);

			if(IsHidden == false)
				text.Draw(renderTarget);
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			var text = Get<TextInstance>(TextUID);
			if(text != null)
			{
				text.ParentUID = null;
				text.Destroy(false);
			}
		}
		#endregion
	}
}
