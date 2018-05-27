using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionType
{
	AddThink,
	AddWatch,
	AddAttack,
	AddDodge,
	RemoveThink,
	RemoveWatch,
	RemoveAttack,
	RemoveDodge,
	IncreaseValue,
	DecreaseValue
}

public class HistoryAction
{
	public ActionType actType;
	public ActionType backType;
	int value;
	int index;

	public HistoryAction( ActionType type, ActionType backType )
	{
		this.actType = type;
		this.backType = backType;
	}

}

public class HistoryManager : MonoBehaviour 
{
	public static HistoryManager instance;
	private ArrayList historyList;
	private int index;

	public Action<ActionType> OnRedo;
	public Action<ActionType> OnUndo;

	void Awake()
	{
		instance = this;
		historyList = new ArrayList();
		index = -1;
	}

	public void AddHistory( HistoryAction ha )
	{
		int count = historyList.Count;
		for( int i = index + 1; i < count; i++ )
			historyList.RemoveAt(historyList.Count - 1);
		
		historyList.Add( ha );
		index = historyList.Count - 1;
	}

	public void UndoClick()
	{
		if (index == -1)
			return;
		
		HistoryAction ha = (HistoryAction)historyList[index];

		if ( OnUndo != null )
			OnUndo( ha.backType );

		index--;
	}

	public void RedoClick()
	{
		if (index == historyList.Count - 1 )
			return;

		index++;

		HistoryAction ha = (HistoryAction)historyList[index];

		if ( OnUndo != null )
			OnUndo( ha.actType );

	}
}
