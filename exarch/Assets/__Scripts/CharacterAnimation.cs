using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	// Public boolean for running animation
	public bool _animRun;
	public static bool dead = false;
	// Animator variables
	private Animator anim;
	private string animRun = "Run";
	
	void Start()
	{
		anim = GetComponent<Animator>();	// Get the animator component
	}
	
	void Update()
	{
		if(_animRun && !dead)	// If _animRun is true then continue
		{
			anim.SetBool(animRun, true);	// Set the animator Bool with the String Value of animRun to True
		}
		else	// If _animRun is false then continue
		{
			anim.SetBool(animRun, false);	// Set the animator Bool with the String Value of animRun to True
		}
	}
	
}
