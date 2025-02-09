using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBlock : MonoBehaviour
{
    public enum Animaltype
    {
        Bear,
        Cat,
        Deer,
        Dog,
        Duck,
        Mouse,
        Rabbit,
        End

    }

    [System.Serializable]
    public struct AnimalSprite
    {
        public Animaltype type;
        public Sprite sprite;
    };

    public AnimalSprite[] animalSprites;

    private Animaltype animal;
    public Animaltype animalType
    {
        get { return animal; }
        set { SetAnimalType(value); }
    }

    public int numAnimals
    {
        get { return animalSprites.Length; }
    }


    private SpriteRenderer sprite;
    private Dictionary<Animaltype,Sprite> animals;

    void Awake()
    {
        animals = new Dictionary<Animaltype, Sprite> ();
        sprite = transform.Find("block").GetComponent<SpriteRenderer>();
        for(int i = 0; i < animalSprites.Length; i++)
        {
            if (!animals.ContainsKey(animalSprites[i].type)) {
                animals.Add(animalSprites[i].type, animalSprites[i].sprite);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetAnimalType(Animaltype newAnimal)
    {
        animal = newAnimal;

        if (animals.ContainsKey(newAnimal))
        {
            sprite.sprite = animals[newAnimal];
        }
    }
}
