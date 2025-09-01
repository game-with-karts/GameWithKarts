using System.Collections.Generic;
using System;

[Serializable]
public sealed class LoginInfo {
	public string username;
	public string password;
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
	public int user_id;
	public List<TimeRecord> records;
}

[Serializable]
public sealed class UserData {
	public string username;
	public int id;
	public bool is_admin;
}
