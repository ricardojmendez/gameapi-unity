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

	
	// Use this for initialization
	function Start () 
	{
		Debug.Log("Start (JS)");
		Playtomic.Initialise(1339, "e24ff1548e204607");
        Playtomic.Log.View();
                    
        // geoip lookup
        StartCoroutine(LoadGeoIP());
        
        // gamevars lookup
        StartCoroutine(LoadGameVars());
        
        // data lookup
        StartCoroutine(LoadData());
        
        // player levels
        StartCoroutine(ListLevels());
        StartCoroutine(LoadLevel());
        StartCoroutine(RateLevel());
        StartCoroutine(SaveLevel());
        
        // leaderboards
        StartCoroutine(SaveScore());
        StartCoroutine(ListScores());
	}
	
	// Loading the GameVars
	function LoadGameVars():IEnumerator
	{
		yield StartCoroutine(Playtomic.GameVars.Load());
		var response = Playtomic.GameVars.GetResponse("Load");
		
		if(response.Success)
		{
			Debug.Log("GameVars are loaded!");
			
			for(var key in response.Data)
			{
				Debug.Log("GameVar " + key + " = " + response.Data[key]);
			}
			
			// alternatively
			// response.GetValue("varname");
		}
		else
		{
			Debug.Log("GameVars failed because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	// Performing a GeoIP lookup
	function LoadGeoIP():IEnumerator
	{
		Debug.Log("Loading GeoIP lookup");
		yield StartCoroutine(Playtomic.GeoIP.Lookup());
		var response = Playtomic.GeoIP.GetResponse("Lookup");
		
		if(response.Success)
		{
			Debug.Log("Country is: " + response.GetValue("Code") + " - " + response.GetValue("Name"));
		}
		else
		{
			Debug.Log("Lookup failed because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	// Perform a data lookup
	function LoadData():IEnumerator
	{
		yield StartCoroutine(Playtomic.Data.Views());
		var response = Playtomic.Data.GetResponse("Views");
		
		if(response.Success)
		{
			Debug.Log("Data API returned " + response.GetValue("Value"));
		}
		else
		{
			Debug.Log("Lookup failed because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	// Player levels
		function ListLevels():IEnumerator
		{
			yield StartCoroutine(Playtomic.PlayerLevels.List("popular", 1, 10));
			var response = Playtomic.PlayerLevels.GetResponse("List");
			
			if(response.Success)
			{
				Debug.Log("Levels listed successfully: " + response.NumItems + " in total, " + response.Levels.Count + " returned");
				
				for(var i=0; i<response.Levels.Count; i++)
				{
					var level = response.Levels[i];
					Debug.Log(" - " + level.LevelId + ": " + level.Name);
				}
			}
			else
			{
				Debug.Log("Level list failed to load because of " + response.ErrorCode + ": " + response.ErrorDescription);
			}
		}
	
	function LoadLevel():IEnumerator
	{
		yield StartCoroutine(Playtomic.PlayerLevels.Load("4d8930441ea37f0a34002993"));
		var response = Playtomic.PlayerLevels.GetResponse("Load");
		
		if(response.Success)
		{
			Debug.Log("Level loaded successfully: ");
			
			var level = response.Levels[0];
			Debug.Log(" - " + level.LevelId + ": " + level.Name + "\n" + level.Data);
		}
		else
		{
			Debug.Log("Level failed to load beacuse of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	function RateLevel():IEnumerator
	{
		yield StartCoroutine(Playtomic.PlayerLevels.Rate("4d8930441ea37f0a34002993", 8));
		var response = Playtomic.PlayerLevels.GetResponse("Rate");
		
		if(response.Success)
		{
			Debug.Log("Level rated successfully");
		}
		else
		{
			Debug.Log("Level rating failed beacuse of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	
	function SaveLevel():IEnumerator
	{
		var level = new Playtomic_PlayerLevel();
		level.Name = "this is a level";
		level.PlayerName = "Ben";
		level.Data = "Asdfasdfasdfasdf";
		
		yield StartCoroutine(Playtomic.PlayerLevels.Save(level));
		var response = Playtomic.PlayerLevels.GetResponse("Save");
		
		if(response.Success)
		{
			Debug.Log("Level saved!");
		}
		else
		{
			Debug.Log("Level failed to save because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	// leaderboards
	function SaveScore():IEnumerator
	{
		var score = new Playtomic_PlayerScore();
		score.Name = "Ben";
		score.Points = 1000000;
		
		yield StartCoroutine(Playtomic.Leaderboards.Save("highscores", score, true, true, false));
		var response = Playtomic.PlayerLevels.GetResponse("Save");
		
		if(response.Success)
		{
			Debug.Log("Score saved!");
		}
		else
		{
			Debug.Log("Score failed to save because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	function ListScores():IEnumerator
	{
		yield StartCoroutine(Playtomic.Leaderboards.List("highscores", true, "alltime", 1, 20));
		var response = Playtomic.Leaderboards.GetResponse("List");
		
		if(response.Success)
		{
			Debug.Log("Scores listed successfully: " + response.NumItems + " in total, " + response.Scores.Count + " returned");
			
			for(var i=0; i<response.Scores.Count; i++)
				Debug.Log(response.Scores[i].Name + ": " + response.Scores[i].Points);
		}
		else
		{
			Debug.Log("Score list failed to load because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}