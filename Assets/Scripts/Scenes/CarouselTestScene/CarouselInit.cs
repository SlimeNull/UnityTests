using UnityEngine;

namespace NinjaGame
{
    public class CarouselInit : MonoBehaviour
    {
        [field: SerializeField]
        public int ItemCount { get; set; } = 5;

        private void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                var newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newObject.transform.SetParent(transform);

                newObject.AddComponent<CarouselItem>();
            }
        }
    }

}