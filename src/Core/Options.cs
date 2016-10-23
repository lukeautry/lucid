namespace Lucid.Core
{
	public enum Options
	{
		/// <summary>
		/// http://tools.ietf.org/html/rfc856
		/// </summary>
		TransmitBinary = 0,

		/// <summary>
		/// http://tools.ietf.org/html/rfc857
		/// </summary>
		Echo = 1,

		/// <summary>
		/// http://tools.ietf.org/html/rfc671
		/// </summary>
		Reconnect = 2,

		/// <summary>
		/// http://tools.ietf.org/html/rfc858
		/// </summary>
		SuppressGoAhead = 3,

		/// <summary>
		/// Approx Message Size Negotiation
		// https://google.com/search?q=telnet+option+AMSN
		/// </summary>
		Amsn = 4,

		/// <summary>
		/// http://tools.ietf.org/html/rfc859
		/// </summary>
		Status = 5,

		/// <summary>
		/// http://tools.ietf.org/html/rfc860
		/// </summary>
		TimingMark = 6,

		/// <summary>
		/// http://tools.ietf.org/html/rfc563
		/// http://tools.ietf.org/html/rfc726
		/// </summary>
		Rcte = 7,

		/// <summary>
		/// (Negotiate) Output Line Width
		/// https://google.com/search?q=telnet+option+NAOL
		/// http://tools.ietf.org/html/rfc1073
		/// </summary>
		Naol = 8,

		/// <summary>
		/// (Negotiate) Output Page Size
		/// https://google.com/search?q=telnet+option+NAOP
		/// http://tools.ietf.org/html/rfc1073
		/// </summary>
		Naop = 9,

		/// <summary>
		/// http://tools.ietf.org/html/rfc652
		/// </summary>
		Naocrd = 10,

		/// <summary>
		/// http://tools.ietf.org/html/rfc653
		/// </summary>
		Naohts = 11,

		/// <summary>
		/// http://tools.ietf.org/html/rfc654
		/// </summary>
		Naohtd = 12,

		/// <summary>
		/// http://tools.ietf.org/html/rfc655
		/// </summary>
		Naoffd = 13,

		/// <summary>
		/// http://tools.ietf.org/html/rfc656
		/// </summary>
		Naovts = 14,

		/// <summary>
		/// http://tools.ietf.org/html/rfc657
		/// </summary>
		Naovtd = 15,

		/// <summary>
		/// http://tools.ietf.org/html/rfc658
		/// </summary>
		Naolfd = 16,

		/// <summary>
		/// http://tools.ietf.org/html/rfc698
		/// </summary>
		ExtendAscii = 17,

		/// <summary>
		/// http://tools.ietf.org/html/rfc727
		/// </summary>
		Logout = 18,

		/// <summary>
		/// http://tools.ietf.org/html/rfc735
		/// </summary>
		Bm = 19,

		/// <summary>
		/// http://tools.ietf.org/html/rfc732
		/// http://tools.ietf.org/html/rfc1043
		/// </summary>
		Det = 20,

		/// <summary>
		/// http://tools.ietf.org/html/rfc734
		/// http://tools.ietf.org/html/rfc736
		/// </summary>
		Supdup = 21,

		/// <summary>
		/// http://tools.ietf.org/html/rfc749
		/// </summary>
		SupdupOutput = 22,

		/// <summary>
		/// http://tools.ietf.org/html/rfc779
		/// </summary>
		SendLocation = 23,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1091
		/// </summary>
		TerminalType = 24,

		/// <summary>
		/// http://tools.ietf.org/html/rfc885
		/// </summary>
		EndOfRecord = 25,

		/// <summary>
		/// http://tools.ietf.org/html/rfc927
		/// </summary>
		Tuid = 26,

		/// <summary>
		/// http://tools.ietf.org/html/rfc933
		/// </summary>
		Outmrk = 27,

		/// <summary>
		/// http://tools.ietf.org/html/rfc946
		/// </summary>
		Ttyloc = 28,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1041
		/// </summary>
		Regime3270 = 29,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1053
		/// </summary>
		X3Pad = 30,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1073
		/// </summary>
		Naws = 31,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1079
		/// </summary>
		TerminalSpeed = 32,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1372
		/// </summary>
		ToggleFlowControl = 33,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1184
		/// </summary>
		Linemode = 34,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1096
		/// </summary>
		XDisplayLocation = 35,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1408
		/// </summary>
		Environ = 36,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1416
		/// http://tools.ietf.org/html/rfc2942
		/// http://tools.ietf.org/html/rfc2943
		/// http://tools.ietf.org/html/rfc2951
		/// </summary>
		Authentication = 37,

		/// <summary>
		/// http://tools.ietf.org/html/rfc2946
		/// </summary>
		Encrypt = 38,

		/// <summary>
		/// http://tools.ietf.org/html/rfc1572
		/// </summary>
		NewEnviron = 39,

		/// <summary>
		/// http://tools.ietf.org/html/rfc2355
		/// </summary>
		Tn3270E = 40,

		/// <summary>
		/// https://google.com/search?q=telnet+option+XAUTH
		/// </summary>
		Xauth = 41,

		/// <summary>
		/// http://tools.ietf.org/html/rfc2066
		/// </summary>
		Charset = 42,

		/// <summary>
		/// http://tools.ietf.org/html/draft-barnes-telnet-rsp-opt-01
		/// </summary>
		Rsp = 43,

		/// <summary>
		/// http://tools.ietf.org/html/rfc2217
		/// </summary>
		ComPortOption = 44,

		/// <summary>
		/// http://tools.ietf.org/html/draft-rfced-exp-atmar-00
		/// </summary>
		Sle = 45,

		/// <summary>
		/// http://tools.ietf.org/html/draft-altman-telnet-starttls-02
		/// </summary>
		StartTls = 46,

		/// <summary>
		/// http://tools.ietf.org/html/rfc2840
		/// </summary>
		Kermit = 47,

		/// <summary>
		/// http://tools.ietf.org/html/draft-croft-telnet-url-trans-00
		/// </summary>
		SendUrl = 48,

		/// <summary>
		/// http://tools.ietf.org/html/draft-altman-telnet-fwdx-01
		/// </summary>
		ForwardX = 49,
		
		PragmaLogon = 138,
		SspiLogon = 139,
		PragmaHeartbeat = 140,
		Exopl = 255
	}
}
