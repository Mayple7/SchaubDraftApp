using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DraftFactsTicker : MonoBehaviour
{
	// Array for facts to show on the facts ticker.
	private List<string> factsList = new List<string>
	{
		"The Schaub Keeper League is entering its 16th season!",
		"Franchise Player: Tom Brady is the only player who has been kept by the same team in every season (Kopman).",
		"Long Lost Twins: In a legendary league post in 2006, Sheebs pointed out that Patrick looks a lot like Robbie Gould.",
		"Is This Rigged? Ever since the league went to multiple divisions in 2010, the next 7 champions have come from the second division (Dan, Doug, Patrick, Sheebs).",
		"Ties are an embarrassment to everyone involved. The most ties in one season go to Sheebs, Trevor and Jake with 2 ties each.",
		"Award Ceremony: The 2010 season saw the inauguration of the coveted Schaub Jersey that is awarded to the winner of the league each year.",
		"Flexibility Is Key: The 2012 season was the first season with flex spots in rosters. This was a heavily debated topic in previous years.",
		"Everyone changing team names every year made it a pain to compile stats... Grrrr...",
		"The Greatest Disappointment: One of the greatest ties in league history came after a week-long trash talking tirade between Dan and Sheebs.",
		"Zac Stacy: The buy-out rule for keepers was introduced in the 2016 season and was used 6 times.",
		"Big Money! The most points ever scored in a season was by Doug in 2014 with 1605 points.",
		"Winner Winner Chicken Dinner: Patrick has the highest winning rate of all teams with a 0.5933.",
		"Multiple Personalities: Patrick holds the record for the best and worst season with records of 11-1-1 and 1-12.",
		"I Like To Move It Move It: Sheebs has the highest number of moves with 37. He also has the highest average moves with 26.09",
		"Expert Drafter: Doug has the 3 lowest moves from a championship team with 4, 5 and 7 moves.",
		"2 For The Money: Ben and Patrick are the only players with consecutive championship seasons.",
		"How unlucky! Trevor has the season with the most points against with 1462 points against.",
		"Home Field Advantage: Patrick has the largest favorable average point differential with 67.27 points per season.",
		"Between a Rock And A Hard Place: Hans has the worst average point differential with -114.77 points per season.",
		"One For One: Doug has a perfectly average win/loss ratio. Jake does too, but Doug has played for 9 more seasons.",
		"Always A Bridesmaid Never A Bride: Kopman has been to the most championship games without winning the big one with 3 appearances.",
		"Strong Starter: After Ben won the first two championships, he has yet to see a top 3 finish.",
		"Carry To Victory: The highest points from a single player on a championship is 475 points from Aaron Rodgers on Dan's team in 2011.",
		"Wide Variety: 125 different players have been part of championship rosters.",
		"He's A Winner: Antonio Gates has been part of the most championship teams with 5 appearances. Steve Smith Sr. is second with 4 appearances.",
		"Right Out The Gate: Drew and Jake both made the playoffs their first year in the league. Impressive considering that neither had access to the top 30 players.",
		"Top Of The Class: Of all the people in the third division, Parks has the best winrate at 0.548.",
		"The Runway: After a surprising pick in the 15th round of the 2007 draft, Trevor selected Jamal Lewis and thus The Jamal Lewis Runway was born.",
		"One and Done: After only playing in the first year of the league, Sid was decimated and finished with a 2-12 record and never came back for a second.",
		"Hold 'Em: Only two teams have ended the year with zero moves, Dan and Kopman both in the 2006 season. Dan ended the season 4th, and Kopman 6th.",
		"Low Roller: Ben ended the 2018 season with the least points in league history with 969 points. Parks was 1 point away from matching that low number.",
		"All Or Nothing: Doug has made top 3 only three times, yet with each appearance he wins the championship.",
		"Looking For The First: Colin and Drew are the only two teams without a top 3 appearance. Both players have also only been to the playoffs once each.",
		"Above Average: Kopman has only one season where he had less than 6 wins. Kopman in 2006 with 5 wins, Patrick was close but his 2018 season gave him his second under 6 win season.",
		"Easy Schedules: The least average points against is Parks with 1212.57, Sheebs comes to a close second with 1217.81 points against.",
		"Close But Not Quite: Kopman and Hans have the most top 3 finishes without winning a championship.",
		"Not Quite Elite: Trevor and Jake are the only players who have only won a single championship.",
		"Ode To Patricia: You are my sweet flower. The pollen that resides within your soul fills me with inspiration. The pedals decorated around your beautiful exterior speak to me the way words never will. While I would like to say that I'm victorious by myself, alas, I'm nothing without you, my Patricia. This is my ode to you dear Patricia. May next year bring better fortunes to your gentle garden.",
		"Called It! In a bold statement made to the league on July 30th 2011, Dan predicted his championship run 1 month before the draft.",
		"Speak Softly And Carry A Big Stick: Doug has only made one post season speech after winning the championship 3 times.",
		"Don't Underestimate Them: In 2013, the numer one seed Parks, had a clear advantage going into the first week of the playoffs only to be defeated after a career game from Dan Bailey.",
		"Big Talker: Aside from the original 8 players, Parks has been the most talkative on the league boards giving rival trash-talkers Patrick and Dan a run for their money.",
		"The FAAB Five: While the FAAB ruleset for waiver pickups were first used in the 2016 season, the rule came from new player Drew in a league discussion in 2015.",
		"Smack Talk: Trash talking and smack talking is a big part of our league. Yahoo removed the default trash talking UI in 2015 putting a damper on general trash talking throughout the season. This has been revived in our discord group in 2017!",
		"Boy Who Cried Wolf: After calling Sheebs out for an epic rival match in 2015, Patrick was only able to put up a measily 72 points. This was Patrick's worst game of the season.",
		"Highs and Lows: The most points scored in a week was Kopman in 2007 with 184 points in week 11 against Dan. The least points scored by a full roster was 29 points by Hans against Sheebs in week 10 of the 2009 season.",
		"It's A Shootout! The most combined points scored in a single game was Dan and Kopman with a combined 300 points. Kopman won the high scoring affair 184-116 in week 11 of the 2007 season.",
		"Championship Records: The most points scored in a championship game was Patrick with 158 points in the 2016 title game.",
		"Work Work Work: This draft app took 2 weekends to create. While coming up with these little tidbits of league history took 2 full weeks. 2018's update took about 2 weeks of work to get upgraded!",
		"Rivalry Weeks! The first year with rivalry weeks was the 2009 season which had only 1 rivalry week. The 2015 season however, has the first instance of inter-division only games in the last 3 weeks of the season.",
		"Purple Drank: Hans is the perennial Vikings drafter by drafting players that would normally go undrafted in standard leagues.",
		"A Winner's Plea: The contract system of handling keepers was developed after Doug said it was too obvious to pick keepers each year.",
		"Trade's Ahoy! With new clarification on how extra keepers or less keepers are dealt with, perhaps more trades involving the top players will be made.",
		"The Worst Person: Parks is a Packer fan. That makes him the worst. Yabbs too, he sucks.",
		"Cross Country! With players in Florida, New York and Washington, our league has players all across the country!",
		"Close But No Cigar: Sheebs missed his chance for his first league championship falling to Jake by only 2 points!",
		"Question: Is Joe Flacco ELITE?",
		"Never Let You Go: 2018 may be the first year where Kopman doesn't keep Tom Brady.",
		"Lots Of Facts! There are over 50 bits of information and history being displayed here. Don't miss any of them!",
		"Too Late? Parks finally achieved his dream of acquiring Aaron Rodgers. Everyone is proud of him.",
		"Sleeping Soundly: Colin can see what every NFL player sleeps on, as long as it's a Sleep Number (TM) (R) bed.",
		"Out of Luck! After spending all season feverishly trying to trade away Luck during the 2018 season, Kopman became the poor sap stuck with him.",
		"The end of the tie era: Ties have been eliminated from the league since switching to half PPR.",
		"Ultimate Loser: Dan has the longest losing streak at 12 games in 2018. The very next year he won his third championship!",
		"Wheelin' and Dealin': Sheebs and Parks have made the most trades with 8 trades each! Dan is close behind at 7.",
		"We'll do it live! 2018 had the most trades in a year with 6 trades! The next closest was in 2013 with 3 trades.",
		"A light Brees: In 2018 Jake bought out Brees' contract and had his worst season to date. Colin was the fortunate one to pick him up and got himself a championship.",
		"The little division that could: Drew frantically tried to prove to the league that their division was the best by cherry-picking stats.",
		"LONG LONG MAAAAAN: Long Sample was discovered on Madden and is now a legendary player.",
		"Dying on a Hill: In a discord message from 2018, Yabbs declared that KJ Hill will be a fantasy relevant player in 4 years.",
		"Did you know, that Final Fantasy 14 has a free trial up to level 60 which includes the critically acclaimed Heavensward expansion pack?",
		"RIP Old Divisions: 2022 is the first year with new divisions! Hopefully some epic new rivalries form!",
		"HELP! I'm stuck writing these dumb ticker facts. SAVE MEEEEEEE!!!!"
	};

	private List<string> usedFactsList = new List<string>();
	int currentFactIndex = 0;

	// Time between facts
	public float timeBetweenFacts = 5;
	public float factsScrollSpeed = 5;  // Arbitrary for now

	private enum FactTickerState
	{
		AnimateTicker,
		WaitToShowNext,
		ChangeText
	}
	private FactTickerState showingFact = FactTickerState.WaitToShowNext;
	private float currentTimer = 0;
	private float fullAnimationTime = 2;

	private RectTransform rectTransform;
	private Text textComponent;
	private float canvasScaleFactor;
	private Vector3 hiddenPosition;

	public Camera cam;

	// Use this for initialization
	void Start ()
	{
		rectTransform = GetComponent<RectTransform>();
		textComponent = GetComponent<Text>();
		canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
		hiddenPosition = rectTransform.position;
		hiddenPosition.y = -20;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (showingFact == FactTickerState.AnimateTicker)
		{
			currentTimer += Time.deltaTime;

			if (currentTimer >= timeBetweenFacts)
			{
				showingFact = FactTickerState.WaitToShowNext;

				// Reset the timer and showing fact state
				currentTimer = 0;
			}
		}
		else if(showingFact == FactTickerState.WaitToShowNext)
		{
			currentTimer += Time.deltaTime;

			if (currentTimer >= fullAnimationTime)
			{
				currentTimer = 0;
				showingFact = FactTickerState.ChangeText;

				// Set the fact to display
				SetNewFactText();

				rectTransform.position = hiddenPosition;
			}
		}
		else
		{
			showingFact = FactTickerState.AnimateTicker;

			// Set the position to the right side of the screen
			float startXPosition = cam.pixelWidth + (rectTransform.rect.width * canvasScaleFactor) / 2;
			float fullDistance = startXPosition * 2;

			// Set the starting position
			Vector3 startPosition = rectTransform.position;
			startPosition.x = startXPosition;
			Vector3 endPosition;

			RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, startPosition, cam, out endPosition);
			endPosition.y = -4.740741f;
			rectTransform.position = endPosition;

			// Time = distance / rate (converting from ms to s)
			fullAnimationTime = fullDistance / (factsScrollSpeed * 1000);

			// Start the tween to move to the left side
			rectTransform.DOMoveX(-endPosition.x, fullAnimationTime).SetEase(Ease.Linear);
		}
	}

	private void SetNewFactText()
	{
		// Pick a random fact
		int factIndex = Random.Range(0, factsList.Count - 1);
		textComponent.text = factsList[factIndex];

		// Remove the fact and add it to the used list
		usedFactsList.Add(factsList[factIndex]);
		factsList.RemoveAt(factIndex);

		// Reset the facts list
		if (factsList.Count == 0)
		{
			factsList.AddRange(usedFactsList);
			usedFactsList.Clear();
		}
	}
}
