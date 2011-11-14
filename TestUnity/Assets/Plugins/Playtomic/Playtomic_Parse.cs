using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Playtomic_Parse : Playtomic_Responder
{
	private static string SECTION;
	private static string SAVE;
	private static string DELETE;
	private static string LOAD;
	private static string FIND;
	
	internal static void Initialise(string apikey)
	{
		SECTION = Playtomic_Encode.MD5("parse-" + apikey);
		SAVE = Playtomic_Encode.MD5("parse-save-" + apikey);
		DELETE = Playtomic_Encode.MD5("parse-delete-" + apikey);
		LOAD = Playtomic_Encode.MD5("parse-load-" + apikey);
		FIND = Playtomic_Encode.MD5("parse-find-" + apikey);
	}
	
	public IEnumerator Save(PFObject pobject)
	{
		string url;
		WWWForm post;
		
		Playtomic_Request.Prepare(SECTION, SAVE, ObjectPostData(pobject), out url, out post);
		
		WWW www = new WWW(url, post);
		yield return www;
		
		var response = Playtomic_Request.Process(www);
	
		if (response.Success)
		{
			var data = (Hashtable)response.JSON;
			pobject.ObjectId = (string)data["id"];
			pobject.CreatedAt = DateTime.Parse((string)data["created"]);
			pobject.UpdatedAt = DateTime.Parse((string)data["created"]);
			response.PObject = pobject;
		}
		
		SetResponse(response, "Save");
	}
	
	public IEnumerator Delete(PFObject pobject)
	{
		string url;
		WWWForm post;
		
		Playtomic_Request.Prepare(SECTION, DELETE, ObjectPostData(pobject), out url, out post);
		
		WWW www = new WWW(url, post);
		yield return www;
		
		var response = Playtomic_Request.Process(www);
		SetResponse(response, "Delete");
	}
	
	public IEnumerator Load(string pobjectid, string classname)
	{
		var pobject = new PFObject();
		pobject.ObjectId = pobjectid;
		pobject.ClassName = classname;
		
		string url;
		WWWForm post;
		
		Playtomic_Request.Prepare(SECTION, LOAD, ObjectPostData(pobject), out url, out post);
		
		WWW www = new WWW(url, post);
		yield return www;
		
		var response = Playtomic_Request.Process(www);
		
		if(response.Success)
		{
			var data = response.JSON;
			
			var po = new PFObject();
			po.ClassName = (string)data["classname"];
			po.ObjectId = (string)data["id"];
			po.Password = (string)data["password"];
			po.CreatedAt = DateTime.Parse((string)data["created"]);
			po.UpdatedAt = DateTime.Parse((string)data["updated"]);
			
			var fields = (Hashtable)data["fields"];
			
			foreach(var key in fields.Keys)
			{
				po.Data.Add((string)key, (string)fields[key]);
			}
						
			response.PObject = po;
		}
		
		SetResponse(response, "Load");

	}
	
	public IEnumerator Find(PFQuery pquery)
	{
		var postdata = new Dictionary<String, String>();
		postdata.Add("classname", pquery.ClassName);
		postdata.Add("limit", pquery.Limit.ToString());
		postdata.Add("order", !string.IsNullOrEmpty(pquery.Order) ? pquery.Order : "created_at");
		
		foreach(var key in pquery.WhereData.Keys)
			postdata.Add("data" + key, pquery.WhereData[key]);

		string url;
		WWWForm post;
		
		Playtomic_Request.Prepare(SECTION, FIND, postdata, out url, out post);
		
		WWW www = new WWW(url, post);
		yield return www;
		
		var response = Playtomic_Request.Process(www);
		
		if(response.Success)
		{
			response.PObjects = new List<PFObject>();
			var array = response.ARRAY;
			
			foreach(Hashtable data in array)
			{
				var po = new PFObject();
				po.ClassName = (string)data["classname"];
				po.ObjectId = (string)data["id"];
				po.Password = (string)data["password"];
				po.CreatedAt = DateTime.Parse((string)data["created"]);
				po.UpdatedAt = DateTime.Parse((string)data["updated"]);
				
				var fields = (Hashtable)data["fields"];
				
				foreach(var key in fields.Keys)
				{
					po.Data.Add((string)key, (string)fields[key]);
				}
								
				response.PObjects.Add(po);
			}
		}
		
		SetResponse(response, "Find");
	}
	
	/**
	 * Turns a ParseObject into data to be POST'd for saving, finding 
	 * @param	pobject		The ParseObject
	 */	
	private static Dictionary<String, String> ObjectPostData(PFObject pobject)
	{
		var postobject = new Dictionary<String, String>();
		postobject.Add("classname", pobject.ClassName);
		postobject.Add("id", (pobject.ObjectId == null ? "" : pobject.ObjectId));
		postobject.Add("password", (pobject.Password == null ? "" : pobject.Password));
		
		foreach(var key in pobject.Data.Keys)
			postobject.Add("data" + key, pobject.Data[key]);
					
		return postobject;
	}
}
