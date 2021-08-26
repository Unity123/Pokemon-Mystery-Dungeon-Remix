using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    int turnNum = 0;
    public List<Pokemon> pokemon = new List<Pokemon>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool isTurnDone = true;
        foreach (Pokemon p in pokemon)
        {
            if (p.isMyTurn == true)
            {
                isTurnDone = false;
            }
        }
        if (isTurnDone)
        {
            DoTurn();
        }
    }
    public void DoTurn()
    {
        foreach (Pokemon p in pokemon)
        {
            p.isMyTurn = true;
        }
        turnNum++;
    }
}
