﻿namespace StdEqpTesting.Model
{
	public struct UserInfo
	{
		public int ID;
		public string username;
		public string? password;
		public UserTypeEnum type;
		public int theme;   //0 - Dark, 1 - Light.
		public string? tag;
	}
}
