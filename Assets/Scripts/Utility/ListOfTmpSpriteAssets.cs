
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Code taken from https://www.youtube.com/watch?v=JdGgrMWIknE

[CreateAssetMenu(fileName = "List of Sprite Assets", menuName = "List of Sprite Assets", order = 0)]
public class ListOfTmpSpriteAssets : ScriptableObject
{
    public List<TMP_SpriteAsset> SpriteAssets;
}