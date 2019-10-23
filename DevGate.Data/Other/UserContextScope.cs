namespace DevGate.Data.Other
{
	/// <summary>
	/// Provides information for the context as to what is performing actions
	/// </summary>
	public class UserContextScope
	{
		/// <summary>
		/// User performing the action
		/// </summary>
		public string Username { get; set; }
	}
}