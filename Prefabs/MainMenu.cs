using SMPL.Graphics;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		public Sprite Background { get; private set; }

		protected override void OnStart()
		{
			Background = new();
		}
		protected override void OnUpdate()
		{
			MainCamera.Position = new();
			MainCamera.Angle = 0;
			MainCamera.Scale = 1;
		}
		protected override void OnStop()
		{

		}
	}
}
