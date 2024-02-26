using System;
using UnityEngine;

namespace NinjaGame
{
    public class CarouselItem : MonoBehaviour
    {
        public virtual void OnItemSelected()
        {
            print($"{gameObject} was selected");
        }
    }
}