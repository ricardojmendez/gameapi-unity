//  This file is part of the official Playtomic API for Unity games.  
//  Playtomic is a real time analytics platform for casual games 
//  and services that go in casual games.  If you haven't used it 
//  before check it out:
//  http://playtomic.com/
//
//  Created by ben at the above domain on 2/25/11.
//  Copyright 2011 Playtomic LLC. All rights reserved.
//
//  Documentation is available at:
//  http://playtomic.com/api/unity
//
// PLEASE NOTE:
// You may modify this SDK if you wish but be kind to our servers.  Be
// careful about modifying the analytics stuff as it may give you 
// borked reports.
//
// If you make any awesome improvements feel free to let us know!
//
// -------------------------------------------------------------------------
// THIS SOFTWARE IS PROVIDED BY PLAYTOMIC, LLC "AS IS" AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class Playtomic_Leaderboards : Playtomic_Responder
{
	public IEnumerator CreatePrivateLeaderboard(string table, bool highest, string permalink)
	{
		var postdata = new WWWForm();
		postdata.AddField("table", table);
		postdata.AddField("highest", highest ? "y" : "n");
		postdata.AddField("permalink", permalink);
		
		var www = new WWW(Playtomic.APIUrl + "v2/leaderboards/create.aspx?swfid=" + Playtomic.GameId + "&js=y", postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "CreatePrivateLeaderboard");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "CreatePrivateLeaderboard");
			yield break;
		}
		
		var results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		
		if(response.Success)
		{
			response.Data.Add("TableId", (string)results["tableid"]);
			response.Data.Add("Name", name);
			response.Data.Add("Bitly", (string)results["Bitly"]);
			response.Data.Add("Permalink", (string)results["Permalink"]);
			response.Data.Add("Highest", highest);
			response.Data.Add("RealName", (string)results["RealName"]);
		}
		
		SetResponse(response, "CreatePrivateLeaderboard");
		yield break;
	}
	
	public IEnumerator LoadPrivateLeaderboard(string tableid)
	{
		var postdata = new WWWForm();
		postdata.AddField("tableid", tableid);
		
		var www = new WWW(Playtomic.APIUrl + "v2/leaderboards/load.aspx?swfid=" + Playtomic.GameId + "&js=y", postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "CreatePrivateLeaderboard");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "CreatePrivateLeaderboard");
			yield break;
		}
		
		var results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		
		if(response.Success)
		{
			response.Data.Add("TableId", tableid);
			response.Data.Add("Bitly", (string)results["Bitly"]);
			response.Data.Add("Permalink", (string)results["Permalink"]);
			response.Data.Add("Name", (string)results["Name"]);
			response.Data.Add("RealName", (string)results["RealName"]);
			response.Data.Add("Highest", (bool)results["Highest"]);
		}
		
		SetResponse(response, "LoadPrivateLeaderboard");
		yield break;
	}
	
	public string GetLeaderboardFromUrl()
	{
		var url = Playtomic.SourceUrl;
		
		if(!url.Contains("?"))
			return null;
		
		if(!url.Contains("leaderboard="))
			return null;
		
			
		var afterq = url.substring(url.indexOf("leaderboard=") + 12);
			
		if(afterq.indexOf("&") > -1)
			afterq = afterq.substring(0, afterq.indexOf("&"));
			   
		if(afterq.indexOf("#") > -1)
		   afterq = afterq.substring(0, afterq.indexOf("#"));
		
		if(afterq.Length == 24)
			return afterq;
		
		return null;
	}
	
	public IEnumerator Save(string table, Playtomic_PlayerScore score, bool highest)
	{
		return Save(table, score, highest, false, false);
	}
	
	public IEnumerator Save(string table, Playtomic_PlayerScore score, bool highest, bool allowduplicates)
	{
		return Save(table, score, highest, allowduplicates, false);
	}
	
	public IEnumerator Save(string table, Playtomic_PlayerScore score, bool highest, bool allowduplicates, bool facebook)
	{
		var postdata = new WWWForm();
		postdata.AddField("table", table);
		postdata.AddField("highest", highest ? "y" : "n");
		postdata.AddField("name", score.Name);
		postdata.AddField("points", score.Points.ToString());
		postdata.AddField("allowduplicates", allowduplicates ? "y" : "n");
		postdata.AddField("auth", Hash(Playtomic.SourceUrl + score.Points));
		postdata.AddField("numfields", score.CustomData.Count.ToString());
		postdata.AddField("fbuserid", string.IsNullOrEmpty(score.FBUserId) ? "" : score.FBUserId);
		postdata.AddField("fb", facebook ? "y" : "n");
		postdata.AddField("url", Playtomic.SourceUrl);
		
		var n = 0;
		
		foreach(var key in score.CustomData.Keys)
		{
			postdata.AddField("ckey" + n, key);
			postdata.AddField("cdata" + n, score.CustomData[key]);
			n++;
		}
		
		var www = new WWW(Playtomic.APIUrl + "v2/leaderboards/save.aspx?swfid=" + Playtomic.GameId + "&js=y", postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "Save");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "Save");
			yield break;
		}
		
		var results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		SetResponse(response, "Save");
		yield break;
	}
	
	public IEnumerator SaveAndList(string table, Playtomic_PlayerScore score, bool highest, string mode, int perpage, bool isglobal)
	{
		return SaveAndList(table, score, highest, false, false);
	}
	
	public IEnumerator SaveAndList(string table, Playtomic_PlayerScore score, bool highest, string mode, int perpage, bool isglobal, bool allowduplicates)
	{
		return SaveAndList(table, score, highest, allowduplicates, false);
	}
	
	public IEnumerator SaveAndList(string table, Playtomic_PlayerScore score, bool highest, string mode, int perpage, bool isglobal, bool allowduplicates, bool facebook, Dictionary<String, String> customfilters)
	{
		var postdata = new WWWForm();
		
		// common data
		postdata.AddField("table", table);
		postdata.AddField("highest", highest ? "y" : "n");
		postdata.AddField("fb", facebook ? "y" : "n");
		
		// save data
		postdata.AddField("name", score.Name);
		postdata.AddField("points", score.Points.ToString());
		postdata.AddField("allowduplicates", allowduplicates ? "y" : "n");
		postdata.AddField("auth", Hash(Playtomic.SourceUrl + score.Points));
		postdata.AddField("numfields", score.CustomData.Count.ToString());
		postdata.AddField("fbuserid", string.IsNullOrEmpty(score.FBUserId) ? "" : score.FBUserId);
		postdata.AddField("url", Playtomic.SourceUrl);
		
		var n = 0;
		
		foreach(var key in score.CustomData.Keys)
		{
			postdata.AddField("ckey" + n, key);
			postdata.AddField("cdata" + n, score.CustomData[key]);
			n++;
		}
		
		// list data
		var numfilters = customfilters == null ? 0 : customfilters.Count;
		
		postdata.AddField("global", isglobal ? "y" : "n");
		postdata.AddField("perpage", perpage);
		postdata.AddField("mode", mode);
		postdata.AddField("numfilters", numfilters);
		
		if(numfilters > 0)
		{
			var fieldnumber = 0;
		    
		    foreach(var key in customfilters.Keys)
		    {
				postdata.AddField("lkey" + fieldnumber, key);
				postdata.AddField("ldata" + fieldnumber, customfilters[key]);
				fieldnumber++;
		    }
		}
	
		WWW www = new WWW(Playtomic.APIUrl + "v2/leaderboards/saveandlist.aspx?swfid=" + Playtomic.GameId + "&js=y", postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "SaveAndList");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "SaveAndList");
			yield break;
		}
				
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		response.Scores = new List<Playtomic_PlayerScore>();
		response.NumItems = 0;
    
		if(response.Success)
		{
			var data = (Hashtable)results["Data"];
			var scores = (ArrayList)data["Scores"];
			var len = scores.Count;
			
			response.NumItems = (int)(double)data["NumScores"];
			
			for(var i=0; i<len; i++)
			{
				Hashtable item = (Hashtable)scores[i];	
				
				var score = new Playtomic_PlayerScore();
				score.Name = WWW.UnEscapeURL((string)item["Name"]);
				score.Points = (int)(double)item["Points"];
				score.SDate = DateTime.Parse((string)item["SDate"]);
				score.RDate = WWW.UnEscapeURL((string)item["RDate"]);
				score.Rank = (long)(double)item["Rank"];
				
				if(item.ContainsKey("CustomData"))
				{
					Hashtable customdata = (Hashtable)item["CustomData"];
	
					foreach(var key in customdata.Keys)
						score.CustomData.Add((string)key, WWW.UnEscapeURL((string)customdata[key]));
				}
				
				response.Scores.Add(score);
			}
		}
		
		SetResponse(response, "SaveAndList");
	}
	
	public IEnumerator List(string table, bool highest, string mode, int page, int perpage, bool facebook)
	{
		return List(table, highest, mode, page, perpage, facebook, null, null);
	}
		
	public IEnumerator List(string table, bool highest, string mode, int page, int perpage, bool facebook, Dictionary<String, String> customfilters)
	{	
		return List(table, highest, mode, page, perpage, facebook, customfilters, null);
	}
	
	
	public IEnumerator List(string table, bool highest, string mode, int page, int perpage, bool facebook, Dictionary<String, String> customfilters, string[] friendslist)
	{
		var numfilters = customfilters == null ? 0 : customfilters.Count;
		var postdata = new WWWForm();
		postdata.AddField("table", table);
		postdata.AddField("mode", mode);
		postdata.AddField("page", page);
		postdata.AddField("perpage", perpage);
		postdata.AddField("numfilters", numfilters);
		
		if(numfilters > 0)
		{
			var fieldnumber = 0;
		    
		    foreach(var key in customfilters.Keys)
		    {
				postdata.AddField("ckey" + fieldnumber, key);
				postdata.AddField("cdata" + fieldnumber, customfilters[key]);
				fieldnumber++;
		    }
		}
		
		string url;
		
		if(facebook)
		{
			if(friendslist != null && friendslist.Length > 0)
			{
				postdata.AddField("friendslist", string.Join(",", friendslist));
			}
			
			url = Playtomic.APIUrl + "v2/leaderboards/listfb.aspx?swfid=" + Playtomic.GameId + "&js=y";
		}
		else
		{
			url = Playtomic.APIUrl + "v2/leaderboards/list.aspx?swfid=" + Playtomic.GameId + "&js=y";
		}
		
		WWW www = new WWW(url);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "List");
			yield break;
		}
		
		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "List");
			yield break;
		}
				
		Hashtable results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		response.Scores = new List<Playtomic_PlayerScore>();
		response.NumItems = 0;
    
		if(response.Success)
		{
			var data = (Hashtable)results["Data"];
			var scores = (ArrayList)data["Scores"];
			var len = scores.Count;
			
			response.NumItems = (int)(double)data["NumScores"];
			
			for(var i=0; i<len; i++)
			{
				Hashtable item = (Hashtable)scores[i];	
				
				var score = new Playtomic_PlayerScore();
				score.Name = WWW.UnEscapeURL((string)item["Name"]);
				score.Points = (int)(double)item["Points"];
				score.SDate = DateTime.Parse((string)item["SDate"]);
				score.RDate = WWW.UnEscapeURL((string)item["RDate"]);
				score.Rank = (long)(double)item["Rank"];
				
				if(item.Contains("FBUserId"))
					score.FBUserId = (string)item["FBUserId"];
				
				if(item.ContainsKey("CustomData"))
				{
					Hashtable customdata = (Hashtable)item["CustomData"];
	
					foreach(var key in customdata.Keys)
						score.CustomData.Add((string)key, WWW.UnEscapeURL((string)customdata[key]));
				}
				
				response.Scores.Add(score);
			}
		}
		
		SetResponse(response, "List");
	}
		
	private static string Hash(string input)
	{
        MD5 md5 = MD5.Create();
        byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
		
        StringBuilder sb = new StringBuilder();
		
        for (int i = 0; i < data.Length; i++)
            sb.Append(data[i].ToString("x2"));
		
        return sb.ToString();
    }		
}