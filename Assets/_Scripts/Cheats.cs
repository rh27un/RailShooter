using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cheats : MonoBehaviour
{
    protected List<List<KeyCode>> sequences = 
        new List<List<KeyCode>>() 
        { 
            new List<KeyCode>() { KeyCode.W, KeyCode.W, KeyCode.S, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.A, KeyCode.D, KeyCode.Q, KeyCode.Space } ,
            new List<KeyCode>() { KeyCode.A, KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A, KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.Q, KeyCode.Space }
        };

    protected List<int> validSequences = new List<int>();
    protected List<int> allSequences = new List<int>();
    protected int nextInSequence = 0;
    protected float lastKeyPress;
    [SerializeField]
    protected float cheatTimeOut = 1f;

    [SerializeField]
    protected List<Gun> gunList = new List<Gun>(); 


    protected PlayerHealth health;
    protected Shooting shooting;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<PlayerHealth>();
        shooting = GetComponent<Shooting>();
        for(int i = 0; i < sequences.Count; i++)
		{
            validSequences.Add(i);
            allSequences.Add(i);
		}
    }

    // Update is called once per frame
    void Update()
    {
        List<int> nextValidSequences = new List<int>();
        bool validInput = false;

        for(int i = 0; i < sequences.Count; i++) // iterate through all sequences of key presses
		{
            if (!validSequences.Contains(i)) // skip sequences that have already been broken
                continue;

			if (Input.GetKeyDown(sequences[i][nextInSequence]))
            { 
                if (nextInSequence < sequences[i].Count - 1) 
                {
                    // valid input for sequence i, but could also be valid for another sequence
                    validInput = true;
                    nextValidSequences.Add(i);
                }
                else
				{
                    // valid final input, cheat code is finished
                    DoCheat(i);
                    nextInSequence = 0;
                    validSequences = allSequences;
                    validInput = false; 
                    break; // no need to check other codes
                }
			}
		}

        if (validInput)
        {
            validSequences = nextValidSequences;
            nextInSequence++;
            lastKeyPress = Time.time;
        }
        else if (Input.anyKeyDown || Time.time >= lastKeyPress + cheatTimeOut)
		{
            nextInSequence = 0;
            validSequences = allSequences;
		}
    }

    public void DoCheat(int cheat)
	{
		switch (cheat)
		{
            case 0:
                health.Heal(10000f, true);
                break;
            case 1:
                foreach (Gun gun in gunList) {
                    var gunClone = Instantiate(gun);
                    gunClone.name = gunClone.name.Replace("(Clone)", string.Empty);
                    gunClone.infiniteAmmo = true;
                    gunClone.curAmmo = gunClone.maxAmmo;
                    gunClone.storedAmmo = 999;
                    shooting.LoadWeapon(gunClone);
                }
                break;

        }
	}
}
