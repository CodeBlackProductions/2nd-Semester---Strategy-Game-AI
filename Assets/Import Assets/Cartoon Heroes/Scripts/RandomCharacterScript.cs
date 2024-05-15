using UnityEngine;

[RequireComponent(typeof(CartoonHeroes.SetCharacter))]
public class RandomCharacterScript : MonoBehaviour
{
    [SerializeField] private int group0Count;
    [SerializeField] private int group1Count;
    [SerializeField] private int group2Count;
    [SerializeField] private int group3Count;

    private void OnEnable()
    {
        CartoonHeroes.SetCharacter characterScript = GetComponent<CartoonHeroes.SetCharacter>();

        characterScript.AddItem(characterScript.itemGroups[0], Random.Range(0, group0Count - 1));
        characterScript.AddItem(characterScript.itemGroups[1], Random.Range(0, group1Count - 1));
        characterScript.AddItem(characterScript.itemGroups[2], Random.Range(0, group2Count - 1));
        characterScript.AddItem(characterScript.itemGroups[3], Random.Range(0, group3Count - 1));
    }
}