using System;
using UnityEngine;

namespace UnityTests
{
    public class CarouselItem : MonoBehaviour
    {
        public virtual void OnItemSelected()
        {
            print($"{gameObject} was selected");
        }
    }
}