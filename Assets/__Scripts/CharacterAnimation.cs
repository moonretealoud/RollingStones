using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	// Public boolean for running animation
	public bool _animRun;
	public static bool dead = false;
	// Animator variables
	private Animator anim;
	private readonly string animRun = "Sprint";
	private readonly string animDeath = "Death";

	void Start()
	{
		anim = GetComponent<Animator>();	// Get the animator component
	}
	
	void Update()
	{
		if (_animRun && MovementRealize.canMove)  // If _animRun is true then continue
		{
			anim.SetBool(animRun, true);
			anim.SetBool(animDeath, false); // Set the animator Bool with the String Value of animRun to True
		}
		else if (!MovementRealize.canMove)   // If _animRun is false then continue
		{
			anim.SetBool(animRun, false);   // Set the animator Bool with the String Value of animRun to True
			anim.SetBool(animDeath, true);
		}
		else
		{
            anim.SetBool(animRun, false);
			anim.SetBool(animDeath, false);
        }
	}

}
