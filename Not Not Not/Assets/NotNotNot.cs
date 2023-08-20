using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class NotNotNot : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMNeedyModule Needy;
   //public GameObject[] Holders;
   //public Material[] Colors;

   public KMSelectable YesButton;
   public KMSelectable NoButton;
   public TextMesh Display;

   bool IsAnswerTrue = false;
   bool SelectedAnswer;
   string Question = "";
   bool IsThisFirstQuestion = true;
   bool IsNeedyActive = false;

   string[] KeepAnswerQuestions = { "What was your\nlast answer?",
"If I were to ask what was\nyour last answer, what would\nyou respond with?",
"If I were to ask what wasn't\nyour last answer, what\nwould you not respond with?",
"What wasn't the button\nthat you didn't\npress last time?",
"What was the button\nyou pressed last?"
   };

   string[] ChangeAnswerQuestions = {
      "What was not\nyour last answer?",
"If I were to ask what was\nyour last answer, what would\nyou not respond with?",
"If I were to ask what wasn't\nyour last answer, what\nwould you respond with?",
"What was the button\nthat you didn't\npress last time?",
"What wasn't the\nbutton you\npressed last?"
   };

   int[] FontSizes = { 300, 200, 210, 250, 290 };
   int QuestionIndex;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
      ModuleId = ModuleIdCounter++;
      Needy.OnNeedyActivation += OnNeedyActivation;
      Needy.OnNeedyDeactivation += OnNeedyDeactivation;
      Needy.OnTimerExpired += OnTimerExpired;


      YesButton.OnInteract += delegate () { YesPress(); return false; };
      NoButton.OnInteract += delegate () { NoPress(); return false; };

   }
   
   void YesPress () {
      YesButton.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, YesButton.transform);
      if (!IsNeedyActive) {
         return;
      }
      if (IsAnswerTrue) {
         OnNeedyDeactivation();
      }
      else {
         Strike();
         OnNeedyDeactivation();
      }
      IsAnswerTrue = true;
   }

   void NoPress () {
      NoButton.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, NoButton.transform);
      if (!IsNeedyActive) {
         return;
      }
      if (!IsAnswerTrue) {
         OnNeedyDeactivation();
      }
      else {
         Strike();
         OnNeedyDeactivation();
      }
      IsAnswerTrue = false;
   }
      /*
       * 
   void YesPress () {
      if (!IsNeedyActive) {
         return;
      }
      SelectedAnswer = true;
      Holders[0].GetComponent<MeshRenderer>().material = Colors[1];
      Holders[1].GetComponent<MeshRenderer>().material = Colors[0];
   }

   void NoPress () {
      if (!IsNeedyActive) {
         return;
      }
      SelectedAnswer = false;
      Holders[0].GetComponent<MeshRenderer>().material = Colors[0];
      Holders[1].GetComponent<MeshRenderer>().material = Colors[1];
   }
   */
   protected void OnNeedyActivation () { //Shit that happens when a needy turns on.
      IsNeedyActive = true;
      if (IsThisFirstQuestion) {
         if (Rnd.Range(0, 2) == 0) {
            GenerateInitalTrueQuestion();
         }
         else {
            GenerateInitialFalseQuestion();
         }
         IsThisFirstQuestion = false;
         ChangeQuestion(Question);
         return;
      }
      else {
         if (Rnd.Range(0, 2) == 0) {
            GenerateKeepAnswerQuestion();
         }
         else {
            GenerateChangeAnswerQuestion();
         }
      }
      ChangeFontSize();
   }

   void ChangeFontSize () {
      Display.fontSize = FontSizes[QuestionIndex];
   }

   protected void OnNeedyDeactivation () { //Shit that happens when a needy turns off.
      IsNeedyActive = false;
      Needy.OnPass();
   }

   protected void OnTimerExpired () { //Shit that happens when a needy turns off due to running out of time.
      Strike();
      OnNeedyDeactivation();
   }

   void GenerateKeepAnswerQuestion () {
      QuestionIndex = Rnd.Range(0, KeepAnswerQuestions.Length);
      ChangeQuestion(KeepAnswerQuestions[QuestionIndex]);
   }

   void GenerateChangeAnswerQuestion () {
      IsAnswerTrue = !IsAnswerTrue;
      QuestionIndex = Rnd.Range(0, ChangeAnswerQuestions.Length);
      ChangeQuestion(ChangeAnswerQuestions[QuestionIndex]);
   }

   void GenerateInitalTrueQuestion () {
      IsAnswerTrue = true;
      switch (Rnd.Range(0, 5)) {
         case 0:
            Question = "Is this module called\nnot not not?";
            break;
         case 1:
            Question = "Does the bomb have\n" + Bomb.GetBatteryCount().ToString() + " batteries?";
            break;
         case 2:
            Question = "Does the bomb have\n" + Bomb.GetBatteryHolderCount().ToString() + " batteries holders?";
            break;
         case 3:
            Question = "Does the bomb have\n" + Bomb.GetIndicators().Count().ToString() + " indicators?";
            break;
         case 4:
            Question = "Does the bomb have\n" + Bomb.GetPortCount().ToString() + " ports?";
            break;
         default:
            break;
      }
   }

   void GenerateInitialFalseQuestion () {
      IsAnswerTrue = false;
      int Rand = 0;
      switch (Rnd.Range(0, 5)) {
         case 0:
            Question = "Is this module called\nNeeDeez Nots?";
            break;
         case 1:
            do {
               Rand = Rnd.Range(0, 11);
            } while (Bomb.GetBatteryCount() == Rand);
            Question = "Does the bomb have\n" + Rand.ToString() + " batteries?";
            break;
         case 2:
            do {
               Rand = Rnd.Range(0, 6);
            } while (Bomb.GetBatteryHolderCount() == Rand);
            Question = "Does the bomb have\n" + Rand.ToString() + " batteries holders?";
            break;
         case 3:
            do {
               Rand = Rnd.Range(0, 6);
            } while (Bomb.GetIndicators().Count() == Rand);
            Question = "Does the bomb have\n" + Rand.ToString() + " indicators?";
            break;
         case 4:
            do {
               Rand = Rnd.Range(0, 7);
            } while (Bomb.GetPortCount() == Rand);
            Question = "Does the bomb have\n" + Rand.ToString() + " ports?";
            break;
         default:
            break;
      }
   }

   void ChangeQuestion (string Input) {
      Display.text = Input;
   }

   void Strike () {
      GetComponent<KMNeedyModule>().HandleStrike();
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} Yes/No to press that button.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper();
      yield return null;
      if (Command == "YES") {
         YesButton.OnInteract();
         yield break;
      }
      if (Command == "NO") {
         NoButton.OnInteract();
         yield break;
      }
   }

   void TwitchHandleForcedSolve () {
      StartCoroutine(HandleAutosolve());
   }

   IEnumerator HandleAutosolve () {
      while (true) {
         while (!IsNeedyActive) yield return null;
         if (IsAnswerTrue) {
            YesButton.OnInteract();
         }
         else {
            NoButton.OnInteract();
         }
      }
   }

}
