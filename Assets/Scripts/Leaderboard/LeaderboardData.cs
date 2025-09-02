using System.Collections.Generic;
using System;

[Serializable]
public sealed class LoginRequest {
	public string username;
	public string password;
}

[Serializable]
public sealed class LoginInfo {
	public string username;
	public string token;
}

[Serializable]
public sealed class TimeRecord {
	public string track;
	public int time;
	public int lap1;
	public int lap2;
	public int lap3;
}

[Serializable]
public sealed class Records {
	public int version;
	public List<TimeRecord> records;
}

