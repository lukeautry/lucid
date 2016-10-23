namespace Lucid.Core
{
	/// <summary>
	/// https://tools.ietf.org/html/rfc854
	/// </summary>
	public enum Commands
	{
		/// <summary>
		/// End of subnegotiation parameters
		/// </summary>
		Se = 240,

		/// <summary>
		/// No operation.
		/// </summary>
		Nop = 241,

		/// <summary>
		/// The data stream portion of a Synch. This should always be accompanied by a TCP Urgent notification.
		/// </summary>
		DataMark = 242,

		/// <summary>
		/// NVT character BRK.
		/// </summary>
		Break = 243,

		/// <summary>
		/// The function IP.
		/// </summary>
		InterruptProcess = 244,

		/// <summary>
		/// The function AO.
		/// </summary>
		AbortOutput = 245,

		/// <summary>
		/// The function AYT.
		/// </summary>
		AreYouThere = 246,

		/// <summary>
		/// The function EC.
		/// </summary>
		EraseCharacter = 247,

		/// <summary>
		/// The function EL.
		/// </summary>
		EraseLine = 248,

		/// <summary>
		/// The GA signal.
		/// </summary>
		GoAhead = 249,

		/// <summary>
		/// Indicates that what follows is subnegotiation of the indicated option.
		/// </summary>
		Sb = 250,

		/// <summary>
		/// Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
		/// </summary>
		Will = 251,

		/// <summary>
		/// ndicates the refusal to perform, or continue performing, the indicated option.
		/// </summary>
		Wont = 252,

		/// <summary>
		/// Indicates the request that the other party perform, or confirmation that you are expecting the other party to perform, the indicated option.
		/// </summary>
		Do = 253,

		/// <summary>
		/// Indicates the demand that the other party stop performing, or confirmation that you are no longer expecting the other party to perform, the indicated option.
		/// </summary>
		Dont = 254,

		/// <summary>
		/// Data Byte 255.
		/// </summary>
		Iac = 255
	}
}
