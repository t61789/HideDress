using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EFT.Visual;
using EFT.Interactive;

namespace SkinHide
{
    public class SkinHide : MonoBehaviour
    {
        public SkinDress[] skindress;
        public Dress[] dress;
        public bool skinhide = true;
        public GameObject Player;

        void Update()
        {
            //Destroy LootItem
            if (GetComponent<ObservedLootItem>() == null)
            {
                Debug.LogError("not find ObservedLootItem");
            }
            else if (GetComponent<ObservedLootItem>().enabled == true)
            {
                Destroy(gameObject);
            }

            if (transform.GetComponentInParent<Animator>() == null)
            {
                Debug.LogError("not find Animator");
            }
            else if (skinhide)
            {
                //Get Player GameObject
                Player = transform.GetComponentInParent<Animator>().gameObject;

                //Find Skin
                skindress = Player.GetComponentsInChildren<SkinDress>();
                dress = Player.GetComponentsInChildren<Dress>();

                //False Skin GameObject
                foreach (SkinDress skindress in skindress)
                {
                    skindress.gameObject.SetActive(false);
                }
                foreach (Dress dress in dress)
                {
                    dress.gameObject.SetActive(false);
                }
            }
        }
    }
}

