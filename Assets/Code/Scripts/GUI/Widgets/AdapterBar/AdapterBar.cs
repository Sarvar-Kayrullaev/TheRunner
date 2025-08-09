using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Widget;

namespace Widget
{
    /// <summary>
    /// AdapterBar is a UI component that allows users to switch between different categories or sections.
    /// It contains ItemButtons that can be selected or unselected, changing their appearance based on selection state.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class AdapterBar : MonoBehaviour
    {
        [Header("Selected and Unselected Sprites")]
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite unselectedSprite;
        [Space(1)]
        [Header("Selection Text Colors")]
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color unselectedColor = Color.gray;
        [Space(1)]
        [Header("Adapter Root")]
        [SerializeField] private RectTransform adapterRoot;

        /// <summary>
        /// Indicates whether the adapter has been fully initialized.
        /// "This is used to prevent re-initialization and ensure that the adapter is ready for use."
        /// </summary>
        private bool isFullyInitialized = false;

        private void Start()
        {
            Initialize();
            Select(0);
        }
        private void Initialize()
        {
            foreach (Transform item in adapterRoot)
            {
                if (item.TryGetComponent(out ItemButton button))
                {
                    button.OnClick.AddListener(() => Select(item.GetSiblingIndex()));
                }
                else
                {
                    Debug.LogWarning($"ItemButton component not found on {item.name}. Please ensure it has the ItemButton component attached.");
                }
            }
        }

        /// <summary>
        /// Selects the item at the specified index, updating its appearance and the appearance of other items.
        /// </summary>  
        /// <param name="selectedIndex">The index of the item to select.</param>
        public void Select(int selectedIndex)
        {
            int index = 0;
            foreach (Transform item in adapterRoot)
            {
                ItemButton button = item.GetComponent<ItemButton>();
                if (index == selectedIndex)
                {
                    if (button != null)
                    {
                        button.Selected = true;
                        if (button.TryGetComponent(out UnityEngine.UI.Image image))
                        {
                            image.sprite = selectedSprite;
                        }
                        else
                        {
                            Debug.LogWarning($"Image component not found on {button.name}. Please ensure it has an Image component attached.");
                        }

                        if (button.transform.GetChild(0).TryGetComponent(out TMP_Text text)) text.color = selectedColor;
                        else
                        {
                            Debug.LogWarning($"Text component not found on {button.name}. Please ensure it has a Text component attached to the first child.");
                        }

                        if (isFullyInitialized == false)
                        {
                            isFullyInitialized = true; // Mark as fully initialized after the first selection
                            button.OnClick.Invoke(); // Default action on first selection
                        }
                    }
                }
                else
                {
                    if (button != null)
                    {
                        button.Selected = false;
                        if (button.TryGetComponent(out UnityEngine.UI.Image image))
                        {
                            image.sprite = unselectedSprite;
                        }
                        else
                        {
                            Debug.LogWarning($"Image component not found on {button.name}. Please ensure it has an Image component attached.");
                        }
                        if (button.transform.GetChild(0).TryGetComponent(out TMP_Text text)) text.color = unselectedColor;
                        else
                        {
                            Debug.LogWarning($"Text component not found on {button.name}. Please ensure it has a Text component attached to the first child.");
                        }
                    }
                }
                index++;
            }
        }
    }
}


// public class Dog
// {
//     public string Name { get; set; }
//     public int Age { get; set; }
//     public Dog(string name, int age)
//     {
//         Name = name;
//         Age = age;
//     }
// }

// public class Tech
// {
//     public string Name { get; set; }
//     public string Type { get; set; }
//     public Tech(string name, string type)
//     {
//         Name = name;
//         Type = type;
//     }
// }

// public class Person
// {
//     public string Name { get; set; }
//     public int Age { get; set; }
//     public Person(string name, int age)
//     {
//         Name = name;
//         Age = age;
//     }
// }
// public class CarX
// {
//     public string Model { get; set; }
//     public int Year { get; set; }
//     public CarX(string model, int year)
//     {
//         Model = model;
//         Year = year;
//     }
// }

// public class Adapter {

//     public void BuildAdapter<T>(List<T> items, RectTransform parent, GameObject prefab)
//     {
//         foreach (T item in items)
//         {
//             GameObject instance = Object.Instantiate(prefab, parent);
//             if (instance.TryGetComponent(out UnityEngine.UI.Text text))
//             {
//                 if (item is Dog dog)
//                 {
//                     text.text = $"{dog.Name}, {dog.Age} years old";
//                 }
//                 else if (item is Tech tech)
//                 {
//                     text.text = $"{tech.Name} ({tech.Type})";
//                 }
//                 else if (item is Person person)
//                 {
//                     text.text = $"{person.Name}, {person.Age} years old";
//                 }
//                 else if (item is CarX car)
//                 {
//                     text.text = $"{car.Model}, {car.Year}";
//                 }
//                 else
//                 {
//                     text.text = item.ToString();
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning($"Text component not found on {instance.name}. Please ensure it has a Text component attached.");
//             }
//         }
//     }
// }

// public class AdapterBarExample : MonoBehaviour
// {
//     [SerializeField] private RectTransform adapterRoot;
//     [SerializeField] private GameObject itemPrefab;

//     private void Start()
//     {
//         List<Dog> dogs = new()
//         {
//             new Dog("Buddy", 3),
//             new Dog("Max", 5),
//             new Dog("Bella", 2)
//         };

//         List<Tech> techs = new()
//         {
//             new Tech("Laptop", "Electronics"),
//             new Tech("Smartphone", "Electronics"),
//             new Tech("Tablet", "Electronics")
//         };

//         List<Person> persons = new()
//         {
//             new Person("Alice", 30),
//             new Person("Bob", 25),
//             new Person("Charlie", 35)
//         };

//         List<CarX> cars = new()
//         {
//             new CarX("Toyota", 2020),
//             new CarX("Honda", 2019),
//             new CarX("Ford", 2021)
//         };

//         Adapter adapter = new();
//         adapter.BuildAdapter(dogs, adapterRoot, itemPrefab);
//         adapter.BuildAdapter(techs, adapterRoot, itemPrefab);
//         adapter.BuildAdapter(persons, adapterRoot, itemPrefab);
//         adapter.BuildAdapter(cars, adapterRoot, itemPrefab);
//     }
// }
