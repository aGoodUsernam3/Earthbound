using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class EarthboundScript : MonoBehaviour
{
	public KMAudio audio;
	public KMBombInfo bomb;
	public KMSelectable bashButton;
	public KMSelectable psiButton;
	public KMSelectable defendButton;
	public KMSelectable runButton;

	//Setting Up the Randomness
	public Material[] characterOptions;
	public Renderer character;
	private int characterIndex = 0;

	public Material[] enemyOptions;
	public Renderer enemy;
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




	//Logging
	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved = false;

	void Awake()
	{
		GetComponent<KMBombModule>().OnActivate += FunnySound;
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
		audio.PlaySoundAtTransform("StartUpSound", transform);
	}

	void Start()
	{
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



	void PickCharacter()
	{
		characterIndex = UnityEngine.Random.Range(0,16);
		character.material = characterOptions[characterIndex];
		Debug.LogFormat("[Earthbound #{0}] You are {1} ", moduleId, characterOptions[characterIndex].name);
	}

	void PickEnemy()
	{
		enemyIndex = UnityEngine.Random.Range(0,30);
		enemy.material = enemyOptions[enemyIndex];
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
				if (((characterIndex == 3) || (characterIndex == 6) || (characterIndex == 9) || (characterIndex == 15)) && (bomb.GetSerialNumber().Any(ch => "DIT04".Contains(ch))))
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
				if ((enemyIndex == 26) || (enemyIndex == 27) || (enemyIndex == 28) || (enemyIndex == 16) || (enemyIndex == 21) || (enemyIndex == 17) || (enemyIndex == 1) || (enemyIndex == 2) || (enemyIndex == 3) || (enemyIndex == 5))
				{
					correctNumber += 35;
					Debug.LogFormat("[Earthbound #{0}] Enemy was from Mother 1, so the number is now {1}. ", moduleId, correctNumber);
				} else if ((enemyIndex == 19) || (enemyIndex == 4) || (enemyIndex == 8) || (enemyIndex == 7) || (enemyIndex == 10) || (enemyIndex == 29) || (enemyIndex == 13) || (enemyIndex == 12) || (enemyIndex == 15) || (enemyIndex == 22))
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
				if ((characterIndex == 14) || (characterIndex == 11) || (characterIndex ==  7) || (characterIndex == 0))
				{
					correctNumber += 89;
					Debug.LogFormat("[Earthbound #{0}] Character is from Mother 1, so the number is now {1}. ", moduleId, correctNumber);
				}	else if ((characterIndex == 10) || (characterIndex == 12) || (characterIndex == 13) || (characterIndex == 4))
				{
					correctNumber -= 94;
					Debug.LogFormat("[Earthbound #{0}] Character is from Mother 2, so the number is now {1}. ", moduleId, correctNumber);
				} else if ((characterIndex == 8) || (characterIndex == 5) || (characterIndex == 2) || (characterIndex == 1))
				{
					correctNumber *= 6;
					Debug.LogFormat("[Earthbound #{0}] Character is from Mother 3, so the number is now {1}. ", moduleId, correctNumber);
				} else
				{
					Math.Abs(correctNumber);
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
			Debug.LogFormat("[Earthbound #{0}] (0-100) Solution is to use PSI at XX:X{1}. ", moduleId, Math.Abs(bomb.GetPortCount() - (bomb.GetOnIndicators().Count() + bomb.GetOffIndicators().Count()) * bomb.GetBatteryCount()) % 10);
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
				GetComponent<KMBombModule>().HandlePass();
				audio.PlaySoundAtTransform("Solved", transform);
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
					GetComponent<KMBombModule>().HandlePass();
					audio.PlaySoundAtTransform("Solved", transform);
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
						GetComponent<KMBombModule>().HandlePass();
						audio.PlaySoundAtTransform("Solved", transform);
						moduleSolved = true;
					}
				}
					else
					{
						if (moduleSolved == false)
					{
					GetComponent<KMBombModule>().HandleStrike();
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

		if ((correctRange == 0100) && (((int)Math.Floor(bomb.GetTime()) % 10) == (Math.Abs(bomb.GetPortCount() - (bomb.GetOnIndicators().Count() + bomb.GetOffIndicators().Count()) * bomb.GetBatteryCount()) % 10)))
		{
			if (moduleSolved == false)
			{
				Debug.LogFormat("[Earthbound #{0}] You tried to use PSI at {1}. ", moduleId, (bomb.GetFormattedTime()));
				Debug.LogFormat("[Earthbound #{0}] {1} HP of damage to the {2}! ", moduleId, (values[usedBackgroundInt] * 8), enemyOptions[enemyIndex].name);
				Debug.LogFormat("[Earthbound #{0}] The {1} disappeared! Module solved. ", moduleId, enemyOptions[enemyIndex].name);
				GetComponent<KMBombModule>().HandlePass();
				audio.PlaySoundAtTransform("Solved", transform);
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
						GetComponent<KMBombModule>().HandlePass();
						audio.PlaySoundAtTransform("Solved", transform);
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
								GetComponent<KMBombModule>().HandlePass();
								audio.PlaySoundAtTransform("Solved", transform);
								moduleSolved = true;
							}
						}

						else
						{
							if (moduleSolved == false)
							{
								GetComponent<KMBombModule>().HandleStrike();
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
				GetComponent<KMBombModule>().HandlePass();
				audio.PlaySoundAtTransform("Solved", transform);
				moduleSolved = true;
			}
		}
				else if ((primes[((int)Math.Floor(bomb.GetTime()) % 60)] != true) && (correctRange == 301400))
				{
				if (moduleSolved == false)
				{
					GetComponent<KMBombModule>().HandleStrike();
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
							GetComponent<KMBombModule>().HandlePass();
							audio.PlaySoundAtTransform("Solved", transform);
							moduleSolved = true;
						}
					}
						else
						{
							if (moduleSolved == false)
							{
								GetComponent<KMBombModule>().HandleStrike();
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
			GetComponent<KMBombModule>().HandlePass();
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
				GetComponent<KMBombModule>().HandlePass();
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
						GetComponent<KMBombModule>().HandlePass();
						audio.PlaySoundAtTransform("Solved", transform);
						moduleSolved = true;
					}
				}

				else
			{
				if (moduleSolved == false)
			{
			GetComponent<KMBombModule>().HandleStrike();
			Debug.LogFormat("[Earthbound #{0}] You tried to Run at {1}, but failed. ", moduleId, (bomb.GetFormattedTime()));
			Debug.LogFormat("[Earthbound #{0}] {1} attacks for fatal damage! ", moduleId, enemyOptions[enemyIndex].name);
			Debug.LogFormat("[Earthbound #{0}] {1} got hurt and collapsed. ", moduleId, characterOptions[characterIndex].name);
			Debug.LogFormat("[Earthbound #{0}] {1} lost the battle, module will now strike. ", moduleId, characterOptions[characterIndex].name);
			audio.PlaySoundAtTransform("Strike", transform);
			}
		}
	}
}
