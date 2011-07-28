using System;
using System.Text;
using System.Security.Cryptography;

internal class Playtomic_Encode
{
	public static string MD5(string input)
	{
        var md5 = System.Security.Cryptography.MD5.Create();
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

        var sb = new StringBuilder();
		
        for (var i = 0; i < data.Length; i++)
            sb.Append(data[i].ToString("x2"));
		
        return sb.ToString();
    }
	
	public static string Base64(string data)
	{
        var enc = new byte[data.Length];
        enc = System.Text.Encoding.UTF8.GetBytes(data);    
        return Convert.ToBase64String(enc);
	}
}
