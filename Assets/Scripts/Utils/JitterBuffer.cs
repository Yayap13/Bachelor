using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using UnityEngine;

public class JitterBuffer<T>
{
	private List<T> _buffer = new List<T>();
	private readonly Dictionary<T, int> _elementToSequence = new Dictionary<T, int>();

	private readonly int _elementsToJitter;
	private readonly float _timeBetweenSequences;
	private readonly Func<T, int> _orderMethod;

	private int _startingSequence = 0;
	private float _startingTime = 0;
	private int _lastSequenceReleased = 0;

	public JitterBuffer(int elementsToJitter, int updatePerSeconds, Func<T, int> orderMethod)
	{
		_elementsToJitter = elementsToJitter;
		_timeBetweenSequences = 1f / updatePerSeconds;
		_orderMethod = orderMethod;
	}
	
	public void Add(T element, int sequence)
	{
		if(_lastSequenceReleased > sequence)
			return;

		if (_startingSequence == 0)
		{
			_startingSequence = sequence;
			_startingTime = Time.time + (_timeBetweenSequences*_elementsToJitter);
		}
		
		_buffer.Add(element);
		_elementToSequence.Add(element, sequence);

		if (_orderMethod != null)
		{
			_buffer = _buffer.OrderBy(_orderMethod).ToList();
		}
		
	}

	private float NextSequenceReleaseTime()
	{
		return ((_elementToSequence[_buffer.First()] - _startingSequence) * _timeBetweenSequences) + _startingTime;
	}

	private bool HasSomethingToRelease()
	{
		return _buffer.Count>0 && Time.time>=NextSequenceReleaseTime();
	}

	public bool TryGet(out T element)
	{
		if (HasSomethingToRelease())
		{
			do
			{
				element = _buffer.First();
				_buffer.Remove(element);
			} while (HasSomethingToRelease());

			_lastSequenceReleased = _elementToSequence[element];
			_elementToSequence.Remove(element);
			return true;
		}
		element = default(T);;
		return false;
	}
}
