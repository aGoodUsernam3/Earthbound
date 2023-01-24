using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class EarthboundScript : MonoBehaviour
{
	public KMAudio audio;
	public KMBombInfo bomb;
	public KMBombModule module;
	public KMSelectable bashButton;
	public KMSelectable psiButton;
	public KMSelectable defendButton;
	public KMSelectable runButton;

	//Setting Up the Randomness
	public Sprite[] characterOptions;
	public SpriteRenderer character;
	private int characterIndex = 0;

	public Sprite[] enemyOptions;
	public SpriteRenderer enemy;
	private int enemyIndex = 0;

	public Material[] backgroundOptions;
	public MeshRenderer background;
	private int[] values = {13, 16, 18, 20, 22, 24, 26, 31, 32, 33, 35, 43, 44, 45, 47, 48, 49, 53, 57, 67, 76, 77, 84, 86, 87, 88, 89, 90, 93, 97};
	private List<bool> primes = new List<bool> { false, false, true, true, false, true, false, true, false, false, false, true, false, true, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, true, false, true, false, false, false, false, false, true, false, false, false, true, false, true, false, false, false, true, false, false, false, false, false, true, false, false, false, false, false, true, false};
	private int usedBackgroundInt;
	private int StartingNumber;
	private int correctNumber;
	private bool end;
	private int correctRange;
	private int damageDealt;
	private static bool playSound = false;



	//Logging
	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved = false;

	void Awake()
	{
		module.OnActivate += FunnySound;
			moduleId = moduleIdCounter++;
			/*/foreach (KMSelectable object in keypad)
			{
					KMSelectable pressedObject = object;
					object.OnInteract += delegate () { keypadPress(pressedObject); return false; };
					}/*/
					bashButton.OnInteract += delegate () { PressbashButton(); return false; };
					psiButton.OnInteract += delegate () { PresspsiButton(); return false; };
					defendButton.OnInteract += delegate () { PressdefendButton(); return false; };
					runButton.OnInteract += delegate () { PressrunButton(); return false; };
	}

	void FunnySound()
	{
		//Makes the sound to play on a single module per bomb.
		if (!playSound)
		{
			playSound = true;
			audio.PlaySoundAtTransform("StartUpSound", transform);
		}
	}

	void Start()
	{
		playSound = false;
			//if(characterPicked)
			//{
					PickCharacter();
					PickEnemy();
					PickBackground();
					GetStartingValue();
					StepMaker();
					Submission();
		  //}
	}
	private void OnDestroy()
    {
		playSound = false;
    }
	private void Update()
    {
		background.material.mainTextureOffset += Time.deltaTime * Vector2.one;
    }



	void PickCharacter()
	{
		characterIndex = UnityEngine.Random.Range(0,16);
		character.sprite = characterOptions[characterIndex];
		Debug.LogFormat("[Earthbound #{0}] You are {1} ", moduleId, characterOptions[characterIndex].name);
	}

	void PickEnemy()
	{
		enemyIndex = UnityEngine.Random.Range(0,30);
		enemy.sprite = enemyOptions[enemyIndex];
		Debug.LogFormat("[Earthbound #{0}] You confront the {1} ", moduleId, enemyOptions[enemyIndex].name);
	}

	void PickBackground()
	{
		int random = UnityEngine.Random.Range(0, 30);
		usedBackgroundInt = random;
		background.material = backgroundOptions[usedBackgroundInt];
		Debug.LogFormat("[Earthbound #{0}] Your background number is {1} ", moduleId, backgroundOptions[usedBackgroundInt].name);
	}

	//Starting the Module
	int GetStartingValue()
	{
	Debug.LogFormat("[Earthbound #{0}] Your starting number is {1} ", moduleId, values[usedBackgroundInt] + bomb.GetBatteryCount());
	return values[usedBackgroundInt] + bomb.GetBatteryCount();
	}

	private void StepMaker(){
		for (var i = 1; i < 11; i++){
			if (end) break;
			steps(i);
		}
	}

	private void steps(int step) {
		switch (step) {
			case 1:
				StartingNumber = values[usedBackgroundInt] + bomb.GetBatteryCount();
				correctNumber = StartingNumber;
				if (((characterIndex == 12) || (characterIndex == 13) || (characterIndex == 14) || (characterIndex == 15)) && (bomb.GetSerialNumber().Any(ch => "DIT04".Contains(ch))))
				{
					correctNumber += 15;
					Debug.LogFormat("[Earthbound #{0}] Step 1 applies, so the number is now {1}. ", moduleId, correctNumber);
				}

				break;
			case 2:
				if ((bomb.GetBatteryCount() >= 3) && (correctNumber % 2 == 0))
				{
					correctNumber /= 4;
					Debug.LogFormat("[Earthbound #{0}] Step 2 applies, so the number is now {1}. ", moduleId, correctNumber);
				}

				break;
			case 3:
				if (bomb.GetSerialNumber().Any(ch => "AEIOU".Contains(ch)))
				{
					correctNumber -= 20;
					Debug.LogFormat("[Earthbound #{0}] Step 3 applies, so the number is now {1}. ", moduleId, correctNumber);
				} else
					{
					correctNumber += 50;
					Debug.LogFormat("[Earthbound #{0}] Step 3 did not apply, so the number is now {1}. ", moduleId, correctNumber);
					}

				break;
			case 4:
				if (values[usedBackgroundInt] < 50)
				{
					correctNumber *= 5;
					Debug.LogFormat("[Earthbound #{0}] Step 4 applies, so the number is now {1}. ", moduleId, correctNumber);
				}

				break;
			case 5:
				if ((enemyIndex == 0) || (enemyIndex == 1) || (enemyIndex == 2) || (enemyIndex == 3) || (enemyIndex == 4) || (enemyIndex == 5) || (enemyIndex == 6) || (enemyIndex == 7) || (enemyIndex == 8) || (enemyIndex == 9))
				{
					correctNumber += 35;
					Debug.LogFormat("[Earthbound #{0}] Enemy was from Mother 1, so the number is now {1}. ", moduleId, correctNumber);
				} else if ((enemyIndex == 10) || (enemyIndex == 11) || (enemyIndex == 12) || (enemyIndex == 13) || (enemyIndex == 14) || (enemyIndex == 15) || (enemyIndex == 16) || (enemyIndex == 17) || (enemyIndex == 18) || (enemyIndex == 19))
					{
					correctNumber -= bomb.GetPortCount();
					Debug.LogFormat("[Earthbound #{0}] Enemy was from Mother 2, so the number is now {1}. ", moduleId, correctNumber);
					}		 else if	(bomb.GetPortCount() == 0)
								{
								correctNumber = correctNumber;
								Debug.LogFormat("[Earthbound #{0}] Enemy was from Mother 3, but the number of ports is 0. The number stays {1}. ", moduleId, correctNumber);
								} else {
									correctNumber /= bomb.GetPortCount();
									Debug.LogFormat("[Earthbound #{0}] Enemy was from Mother 3, so the number is now {1}. ", moduleId, correctNumber);
												}

												break;
			case 6:
				correctNumber *= (bomb.GetBatteryCount() + 1);
				Debug.LogFormat("[Earthbound #{0}] Number was multiplied by {1} so the number is now {2}. ", moduleId, bomb.GetBatteryCount() + 1, correctNumber);
				break;
			case 7:
				if ((bomb.IsIndicatorOn("BOB")) || (bomb.IsIndicatorOn("CAR")) || (bomb.IsIndicatorOn("IND")) || (bomb.IsIndicatorOn("SIG")) || (bomb.IsIndicatorOn("NSA")) || (bomb.IsIndicatorOn("MSA")) || (bomb.IsIndicatorOff("BOB")) || (bomb.IsIndicatorOff("CAR")) || (bomb.IsIndicatorOff("IND")) || (bomb.IsIndicatorOff("SIG")) || (bomb.IsIndicatorOff("NSA")) || (bomb.IsIndicatorOff("MSA")))
				{
					correctNumber += ((bomb.GetOnIndicators().Count() + bomb.GetOffIndicators().Count()) * (values[usedBackgroundInt] % 10));
					Debug.LogFormat("[Earthbound #{0}] Step 7 applies, so the number is now {1}. ", moduleId, correctNumber);
				}

				break;
			case 8:
				if ((characterIndex == 0) || (characterIndex == 1) || (characterIndex ==  2) || (characterIndex == 3))
				{
					correctNumber += 89;
					Debug.LogFormat("[Earthbound #{0}] Character is from Mother 1, so the number is now {1}. ", moduleId, correctNumber);
				}	else if ((characterIndex == 4) || (characterIndex == 5) || (characterIndex == 6) || (characterIndex == 7))
				{
					correctNumber -= 94;
					Debug.LogFormat("[Earthbound #{0}] Character is from Mother 2, so the number is now {1}. ", moduleId, correctNumber);
				} else if ((characterIndex == 8) || (characterIndex == 9) || (characterIndex == 10) || (characterIndex == 11))
				{
					correctNumber *= 6;
					Debug.LogFormat("[Earthbound #{0}] Character is from Mother 3, so the number is now {1}. ", moduleId, correctNumber);
				} else
				{
					correctNumber = Math.Abs(correctNumber);
					correctNumber *= 16;
					Debug.LogFormat("[Earthbound #{0}] Character was from Mother 4, so the number is now {1}. ", moduleId, correctNumber);
				}

				break;
			case 9:
			 correctNumber += (bomb.GetBatteryHolderCount());
			 correctNumber -= (bomb.GetSerialNumberLetters().Count() * bomb.GetSerialNumberNumbers().Last());
			 Debug.LogFormat("[Earthbound #{0}] After step 9, the number is now {1}. ", moduleId, correctNumber);
			 break;
			case 10:
				correctNumber = (correctNumber % 1000);
				Debug.LogFormat("[Earthbound #{0}] Your final number is now {1}. ", moduleId, correctNumber);
				break;
			}
		}

		private void Submission()
		{
			if ((correctNumber >= 0) && (correctNumber <= 100))
		{
			correctRange = 0100;
			Debug.LogFormat("[Earthbound #{0}] (0-100) Solution is to use PSI at XX:X{1}. ", moduleId, ((bomb.GetPortCount() - bomb.GetIndicators().Count()) * bomb.GetBatteryCount() + 1000) % 10 );
		}
			else if ((correctNumber >= 101) && (correctNumber <= 200))
		{
			correctRange = 101200;
			Debug.LogFormat("[Earthbound #{0}] (101-200) Solution is to Bash at even minutes. ", moduleId);
		}
			else if ((correctNumber >= 201) && (correctNumber <= 300))
		{
			correctRange = 201300;
			Debug.LogFormat("[Earthbound #{0}] (201-300) Solution is to Run at odd minutes. ", moduleId);
		}
			else if ((correctNumber >= 301) && (correctNumber <= 400))
		{
			correctRange = 301400;
			Debug.LogFormat("[Earthbound #{0}] (301-400) Solution is to Defend at prime seconds. ", moduleId);
		}
			else if ((correctNumber >= 401) && (correctNumber <= 500))
		{
			correctRange = 401500;
			Debug.LogFormat("[Earthbound #{0}] (401-500) Solution is to use PSI at XX:X4. ", moduleId);
		}
			else if ((correctNumber >= 501) && (correctNumber <= 600))
		{
			correctRange = 501600;
			Debug.LogFormat("[Earthbound #{0}] (501-600) Solution is to Bash when the seconds match. ", moduleId);
		}
			else if ((correctNumber >= 601) && (correctNumber <= 700))
		{
			correctRange = 601700;
			Debug.LogFormat("[Earthbound #{0}] (601-700) Solution is to Run at XX:X{1}. ", moduleId, (((values[usedBackgroundInt] - 1) % 9) + 1));
		}
			else if ((correctNumber >= 701) && (correctNumber <= 800))
		{
			correctRange = 701800;
			Debug.LogFormat("[Earthbound #{0}] (701-800) Solution is to Defend when the sum of the seconds is equal to {1}. ", moduleId, (bomb.GetBatteryHolderCount() + 5));
		}
			else if ((correctNumber >= 801) && (correctNumber <= 900))
		{
			correctRange = 801900;
			Debug.LogFormat("[Earthbound #{0}] (801-900) Solution is to use PSI at multiples of 5. ", moduleId);
		}
			else if ((correctNumber >= 901) && (correctNumber <= 999))
		{
			correctRange = 901999;
			Debug.LogFormat("[Earthbound #{0}] (901-999) Solution is to Bash anytime. ", moduleId);
		}
			else if (correctNumber < 0)
		{
			correctRange = 00;
			Debug.LogFormat("[Earthbound #{0}] Since the number is negative, the solution is to Run at XX:X{1} ", moduleId, (Math.Abs(correctNumber) % 10));
		}
	}


//(int) bomb.GetTime() Bomb.GetFormattedTime()

	void PressbashButton()
	{
		bashButton.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

		if ((correctRange == 101200) && ((int)bomb.GetTime()) / 60 % 2 == 0)
		{
			if (moduleSolved == false)
			{
				Debug.LogFormat("[Earthbound #{0}] You tried to Bash at {1}. ", moduleId, (bomb.GetFormattedTime()));
				Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
				Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
				StartCoroutine(EnemyKill());
				moduleSolved = true;
			}
		}
			else if ((correctRange == 501600) && ((int)Math.Floor(bomb.GetTime() % 60 / 10) == ((int)Math.Floor(bomb.GetTime() % 10))))
			{
				if (moduleSolved == false)
				{
					Debug.LogFormat("[Earthbound #{0}] You tried to Bash at {1}. ", moduleId, (bomb.GetFormattedTime()));
					Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
					Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
				StartCoroutine(EnemyKill());
					moduleSolved = true;
				}
			}

				else if (correctRange == 901999)
				{
					if (moduleSolved == false)
					{
						Debug.LogFormat("[Earthbound #{0}] You tried to Bash at {1}. ", moduleId, (bomb.GetFormattedTime()));
						Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
						Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
						StartCoroutine(EnemyKill());
						moduleSolved = true;
					}
				}
					else
					{
						if (moduleSolved == false)
					{
					module.HandleStrike();
					Debug.LogFormat("[Earthbound #{0}] You tried to Run at {1}, but failed. ", moduleId, (bomb.GetFormattedTime()));
					Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
					Debug.LogFormat("[Earthbound #{0}] {1} got hurt and collapsed. ", moduleId, characterOptions[characterIndex].name);
					Debug.LogFormat("[Earthbound #{0}] {1} lost the battle, module will now strike. ", moduleId, characterOptions[characterIndex].name);
					audio.PlaySoundAtTransform("Strike", transform);
					}
				}
	}

	void PresspsiButton()
	{
		psiButton.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

		if ((correctRange == 0100) && (((int)Math.Floor(bomb.GetTime()) % 10) == (((bomb.GetPortCount() - (bomb.GetIndicators().Count())) * bomb.GetBatteryCount()) + 1000) % 10))
		{
			if (moduleSolved == false)
			{
				Debug.LogFormat("[Earthbound #{0}] You tried to use PSI at {1}. ", moduleId, (bomb.GetFormattedTime()));
				Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
				Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
				StartCoroutine(EnemyKill());
				moduleSolved = true;
			}
		}

			else if ((correctRange == 401500) && ((int)Math.Floor(bomb.GetTime()) % 10 == 4))
				{
					if (moduleSolved == false)
					{
						Debug.LogFormat("[Earthbound #{0}] You tried to use PSI at {1}. ", moduleId, (bomb.GetFormattedTime()));
						Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
						Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
						StartCoroutine(EnemyKill());
						moduleSolved = true;
					}
				}

					else if ((correctRange == 801900) && ((int)Math.Floor(bomb.GetTime()) % 5 == 0))
						{
							if (moduleSolved == false)
							{
								Debug.LogFormat("[Earthbound #{0}] You tried to use PSI at {1}. ", moduleId, (bomb.GetFormattedTime()));
								Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
								Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
								StartCoroutine(EnemyKill());
								moduleSolved = true;
							}
						}

						else
						{
							if (moduleSolved == false)
							{
								module.HandleStrike();
								Debug.LogFormat("[Earthbound #{0}] You tried to use PSI at {1}, but failed. ", moduleId, (bomb.GetFormattedTime()));
								Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
								Debug.LogFormat("[Earthbound #{0}] {1} got hurt and collapsed. ", moduleId, characterOptions[characterIndex].name);
								Debug.LogFormat("[Earthbound #{0}] {1} lost the battle, module will now strike. ", moduleId, characterOptions[characterIndex].name);
								audio.PlaySoundAtTransform("Strike", transform);
							}
						}


}

	void PressdefendButton()
	{
		defendButton.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if ((correctRange == 301400) && primes[((int)Math.Floor(bomb.GetTime()) % 60)] == true)
		{
			if (moduleSolved == false)
			{
				Debug.LogFormat("[Earthbound #{0}] You tried to Defend at {1}. ", moduleId, (bomb.GetFormattedTime()));
				Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
				Debug.LogFormat("[Earthbound #{0}] You defended the attack, module solved. ", moduleId);
				module.HandlePass();
				StartCoroutine(FadeBG());
				audio.PlaySoundAtTransform("Solved", transform);
				moduleSolved = true;
			}
		}
				else if ((primes[((int)Math.Floor(bomb.GetTime()) % 60)] != true) && (correctRange == 301400))
				{
				if (moduleSolved == false)
				{
					module.HandleStrike();
					Debug.LogFormat("[Earthbound #{0}] You tried to Defend at {1}, but failed. ", moduleId, (bomb.GetFormattedTime()));
					Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
					Debug.LogFormat("[Earthbound #{0}] {1} got hurt and collapsed. ", moduleId, characterOptions[characterIndex].name);
					Debug.LogFormat("[Earthbound #{0}] {1} lost the battle, module will now strike. ", moduleId, characterOptions[characterIndex].name);
					audio.PlaySoundAtTransform("Strike", transform);
				}
			}
			//(([just the seconds] - [last digit]) 10) / 10 = [tens digit of seconds]
					else if ((correctRange == 701800) && (bomb.GetBatteryHolderCount() + 5 == (((int)Math.Floor(bomb.GetTime() % 10)) + (int)Math.Floor(bomb.GetTime() % 60 / 10))))
					{
						if (moduleSolved == false)
						{
							Debug.LogFormat("[Earthbound #{0}] You tried to Defend at {1}. ", moduleId, (bomb.GetFormattedTime()));
							Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
							Debug.LogFormat("[Earthbound #{0}] You defended the attack, module solved. ", moduleId);
							module.HandlePass();
				StartCoroutine(FadeBG());
				audio.PlaySoundAtTransform("Solved", transform);
							moduleSolved = true;
						}
					}
						else
						{
							if (moduleSolved == false)
							{
								module.HandleStrike();
								Debug.LogFormat("[Earthbound #{0}] You tried to Defend at {1}, but failed. ", moduleId, (bomb.GetFormattedTime()));
								Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
								Debug.LogFormat("[Earthbound #{0}] {1} got hurt and collapsed. ", moduleId, characterOptions[characterIndex].name);
								Debug.LogFormat("[Earthbound #{0}] {1} lost the battle, module will now strike. ", moduleId, characterOptions[characterIndex].name);
								audio.PlaySoundAtTransform("Strike", transform);
							}
						}

	}

	void PressrunButton()
	{
		runButton.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if ((correctRange == 00) && (((int)Math.Floor(bomb.GetTime()) % 10) == ((Math.Abs(correctNumber) % 10))))
		{
			if (moduleSolved == false)
			{
			Debug.LogFormat("[Earthbound #{0}] You tried to Run at {1}. ", moduleId, (bomb.GetFormattedTime()));
			Debug.LogFormat("[Earthbound #{0}] You escaped, module solved. ", moduleId);
			module.HandlePass();
				StartCoroutine(FadeBG());
				audio.PlaySoundAtTransform("Solved", transform);
			moduleSolved = true;
			}
		}
			else if ((correctRange == 201300) && ((((int)Math.Floor(bomb.GetTime()) / 60) % 2 == 1)))
			{
				if (moduleSolved == false)
				{
				Debug.LogFormat("[Earthbound #{0}] You tried to Run at {1}. ", moduleId, (bomb.GetFormattedTime()));
				Debug.LogFormat("[Earthbound #{0}] You escaped, module solved. ", moduleId);
				module.HandlePass();
				StartCoroutine(FadeBG());
				audio.PlaySoundAtTransform("Solved", transform);
				moduleSolved = true;
				}
			}

				else if ((correctRange == 601700) && ((((int)Math.Floor(bomb.GetTime()) % 10 == (((values[usedBackgroundInt] - 1) % 9) + 1)))))
				{
					if (moduleSolved == false)
					{
						Debug.LogFormat("[Earthbound #{0}] You tried to Run at {1}. ", moduleId, (bomb.GetFormattedTime()));
						Debug.LogFormat("[Earthbound #{0}] You escaped, module solved. ", moduleId);
						module.HandlePass();
				StartCoroutine(FadeBG());
				audio.PlaySoundAtTransform("Solved", transform);
						moduleSolved = true;
					}
				}

				else
			{
				if (moduleSolved == false)
			{
			module.HandleStrike();
			Debug.LogFormat("[Earthbound #{0}] You tried to Run at {1}, but failed. ", moduleId, (bomb.GetFormattedTime()));
			Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
			Debug.LogFormat("[Earthbound #{0}] {1} got hurt and collapsed. ", moduleId, characterOptions[characterIndex].name);
			Debug.LogFormat("[Earthbound #{0}] {1} lost the battle, module will now strike. ", moduleId, characterOptions[characterIndex].name);
			audio.PlaySoundAtTransform("Strike", transform);
			}
		}
	}

	//Solve Anims
	IEnumerator FadeBG(bool solve = true)
    {
		yield return new WaitForSeconds(0.5f);
		float delta = 0;
        while (delta < 1)
        {
			delta += 1.25f * Time.deltaTime;
			background.material.SetFloat("_Blend", delta);
			enemy.color = Color.Lerp(Color.white, Color.black, delta);
			yield return null;
        }
		if (solve)
			module.HandlePass();
    }
	IEnumerator EnemyKill()
    {
		audio.PlaySoundAtTransform("hit", transform);
        for (int i = 0; i < 3; i++)
        {
			enemy.enabled = false;
			yield return new WaitForSeconds(0.05f);
			enemy.enabled = true;
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds(1);
		StartCoroutine(FadeBG(false));
		yield return FadeEnemy(Color.white, 0.25f);
		yield return FadeEnemy(Color.black, 0.25f);
		yield return new WaitForSeconds(0.05f);
		enemy.enabled = false;
		audio.PlaySoundAtTransform("Solved", transform);
		module.HandlePass();
    }
	IEnumerator FadeEnemy(Color targetColor, float time)
    {
		Color[] pixels = enemy.sprite.texture.GetPixels();
		float delta = 0;
		while (delta < 1)
		{
			delta += 1f/time * Time.deltaTime;
			yield return null;
			Color[] newPixels = pixels.Select(x => x.a >= 0.8f ? Color.Lerp(x, targetColor, delta) : x).ToArray();
			Texture2D newTxt = new Texture2D(enemy.sprite.texture.width, enemy.sprite.texture.height) { wrapMode = TextureWrapMode.Clamp };
			newTxt.SetPixels(newPixels);
			enemy.sprite = Sprite.Create(newTxt, enemy.sprite.rect, new Vector3(0.5f, 0.5f));
		}
	}
	
	//twitch plays
    #pragma warning disable 414
	private readonly string TwitchHelpMessage = @"Use !{0} press [bash/defend/psi/run] at [TIME] to use any move at a certain time. Valid times are # (last second digit), ## (two seconds digit), and #:XX (last minute digit). Timing of # and ## can be chained, but must has the same type. Use !{0} press [bash/defend/psi/run] to press any button at any time.";
    #pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
	{
		command = command.ToLowerInvariant().Trim();
		Match m = Regex.Match(command, @"^press (bash|defend|psi|run)(?: at(?:((?:\s+(?:[0-5]\d))+)|((?:\s+\d)+)|\s+(\d)\:xx))?$");
		if (m.Success)
		{
			HashSet<int> timesToPress = new HashSet<int>();
			bool singleDigit = m.Groups[3].Success;
			bool isMinute = m.Groups[4].Success;
			bool isSecond = m.Groups[2].Success;
			string[] seconds;
			int groupIndex = 0;
			if (isMinute)
				groupIndex = 4;
			else if (singleDigit)
				groupIndex = 3;
			else if (isSecond)
				groupIndex = 2;
			if (groupIndex != 0)
			{
				seconds = m.Groups[groupIndex].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string time in seconds)
					timesToPress.Add(int.Parse(time));
			}
			yield return null;
			Func<bool> timeCal = null;
			if (isMinute)
				timeCal = () => timesToPress.Contains(((int)(bomb.GetTime() / 60)) % 10);
			else if (singleDigit)
				timeCal = () => timesToPress.Contains(((int)bomb.GetTime()) % 10);
			else if (isSecond)
				timeCal = () => timesToPress.Contains(((int)bomb.GetTime()) % 60);
			else
				timeCal = () => true;
			while (!timeCal())
			{
				yield return new WaitForSeconds(.1f);
				yield return "trycancel";
			}
			switch (m.Groups[1].Value)
			{
				case "bash":
					bashButton.OnInteract();
					break;
				case "defend":
					defendButton.OnInteract();
					break;
				case "psi":
					psiButton.OnInteract();
					break;
				case "run":
					runButton.OnInteract();
					break;
			}
			yield return new WaitForSeconds(.1f);
		}
		else
			yield return "sendtochaterror Invalid command. Use !{1} help to see full commands.";
		yield break;
	}

	IEnumerator TwitchHandleForcedSolve()
    {
		Func<bool> correctTime = null;
		KMSelectable buttonToPress = null;
		switch (correctRange)
        {
			case 0100:
				correctTime = () => ((int)Math.Floor(bomb.GetTime()) % 10) == (Math.Abs(bomb.GetPortCount() - (bomb.GetOnIndicators().Count() + bomb.GetOffIndicators().Count()) * bomb.GetBatteryCount()) % 10);
				buttonToPress = psiButton;
				break;
			case 101200:
				correctTime = () => ((int)bomb.GetTime()) / 60 % 2 == 0;
				buttonToPress = bashButton;
				break;
			case 201300:
				correctTime = () => ((int)Math.Floor(bomb.GetTime()) / 60) % 2 == 1;
				buttonToPress = runButton;
				break;
			case 301400:
				correctTime = () => primes[((int)Math.Floor(bomb.GetTime()) % 60)];
				buttonToPress = defendButton;
				break;
			case 401500:
				correctTime = () => (int)Math.Floor(bomb.GetTime()) % 10 == 4;
				buttonToPress = psiButton;
				break;
			case 501600:
				correctTime = () => (int)Math.Floor(bomb.GetTime() % 60 / 10) == ((int)Math.Floor(bomb.GetTime() % 10));
				buttonToPress = bashButton;
				break;
			case 601700:
				correctTime = () => (int)Math.Floor(bomb.GetTime()) % 10 == (((values[usedBackgroundInt] - 1) % 9) + 1);
				buttonToPress = runButton;
				break;
			case 701800:
				correctTime = () => bomb.GetBatteryHolderCount() + 5 == (((int)Math.Floor(bomb.GetTime() % 10)) + (int)Math.Floor(bomb.GetTime() % 60 / 10));
				buttonToPress = defendButton;
				break;
			case 801900:
				correctTime = () => (int)Math.Floor(bomb.GetTime()) % 5 == 0;
				buttonToPress = psiButton;
				break;
			case 901999:
				correctTime = () => true;
				buttonToPress = bashButton;
				break;
			case 00:
				correctTime = () => ((int)Math.Floor(bomb.GetTime()) % 10) == ((Math.Abs(correctNumber) % 10));
				buttonToPress = runButton;
				break;
        }
		while (!correctTime())
			yield return true;
		if (buttonToPress == null) yield break;
		buttonToPress.OnInteract();
		yield return new WaitForSeconds(.1f);
	}
}
