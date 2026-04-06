using System;
using UnityEngine;

namespace TypTyp.Input
{
    public interface ITypingInputListener
    {
        public event Action<char> OnInputTyped;
    }
}