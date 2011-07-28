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

public class Playtomic_GameVars : Playtomic_Responder
{
	public Playtomic_GameVars()	{ }
	
	public IEnumerator Load()
	{
		var postdata = new WWWForm();
		postdata.AddField("unity", 1);
		
		var www = new WWW(Playtomic.APIUrl + "/gamevars/load.aspx?swfid=" + Playtomic.GameId + "&js=true", postdata);
		yield return www;
		
		if (www.error != null)
		{
			SetResponse(Playtomic_Response.GeneralError(www.error), "Load");
			yield break;
		}

		if (string.IsNullOrEmpty(www.text))
		{
			SetResponse(Playtomic_Response.GeneralError(-1), "Load");
			yield break;
		}
		
		var results = (Hashtable)Playtomic_JSON.JsonDecode(www.text);
		
		var response = new Playtomic_Response();
		response.Success = (int)(double)results["Status"] == 1;
		response.ErrorCode = (int)(double)results["ErrorCode"];
		
		if (response.Success)
		{
			var raw = (ArrayList)results["Data"];
			var len = raw.Count;
			for(var i=0; i<len; i++)
			{
				Hashtable item = (Hashtable)raw[i];	
				var name = WWW.UnEscapeURL((string)item["Name"]);
				var value = WWW.UnEscapeURL((string)item["Value"]);
				response.Data.Add(name, value);
			}
			
			SetResponse(response, "Load");
		}
		
		yield break;
	}
}