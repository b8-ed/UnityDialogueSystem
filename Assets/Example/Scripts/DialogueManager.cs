﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
	
	public Text dialogueText;
	public Image imageText;
	public Animator animator;
	public float waitTime = .01f;
	public float voiceVolume = 1f;


	private Queue<string> sentences;
	private Queue<Sprite> sprites;
	private Queue<AudioClip> voices;
	private AudioSource source;
	private bool betweenChars = true, parsing, finished;
	private string timeString;



	void Start () 
	{
		sentences = new Queue<string>();
		sprites = new Queue<Sprite> ();
		voices = new Queue<AudioClip> ();
		imageText = imageText.GetComponent<Image>();
		source = GetComponent<AudioSource>();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Z)) 
		{
			DisplayNextSentence ();
		}
	}

	public void StartDialogue(Dialogue dialogue)
	{
		animator.SetBool("IsOpen", true);

		voices.Clear ();
		sprites.Clear ();
		sentences.Clear();

		foreach(Sentence sentence in dialogue.sentences)
		{


			foreach (string paragraph in sentence.text)
			{
				sprites.Enqueue (sentence.character.standardExpression);
				sentences.Enqueue(paragraph);
				voices.Enqueue (sentence.character.voice);
			}

		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		imageText.sprite = sprites.Dequeue ();
		string sentence = sentences.Dequeue();
		AudioClip audio = voices.Dequeue ();

		StopAllCoroutines();
		waitTime = 0f;
		finished = false;
		StartCoroutine(TypeSentence(sentence, audio));
	}

	IEnumerator TypeSentence(string sentence, AudioClip audio)
	{
		timeString = "";
		parsing = false;
		dialogueText.text = "";
		foreach(char letter in sentence.ToCharArray())
		{

			if (letter == '[') 
			{
				parsing = true;
			}

			if (parsing) 
			{
				if (letter == ']')
				{
					parsing = false;
					waitTime = float.Parse (timeString) * .001f;
					timeString = "";
				} 

				if (letter != '[' && letter != ']')
				{
					timeString += letter;
				}

			}
			else
			{
				dialogueText.text += letter;
				source.PlayOneShot (audio, voiceVolume);
				yield return new WaitForSeconds(waitTime);
			}

		}
	}



	void EndDialogue()
	{
		animator.SetBool("IsOpen", false);
	}
}