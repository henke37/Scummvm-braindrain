namespace DrainLib.Engines {
	public class VideoState {
		public uint CurrentFrame;
		public uint FrameCount;
		public Rational FrameRate;
		public string FileName;

		public float PlaybackPosition => (float)((int)CurrentFrame / FrameRate);

		public float Length => (float)((int)FrameCount / FrameRate);

		public float Progress => (float)CurrentFrame / FrameCount;
	}
}
