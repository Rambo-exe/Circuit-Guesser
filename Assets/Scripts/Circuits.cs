using UnityEngine;

[CreateAssetMenu(fileName = "NewCircuit", menuName = "Circuit/ScrpObj")]
public class Circuits : ScriptableObject
{
    public string circuitName;
    public Sprite circuitImage;
    public string circuitDescription;
    public string hint;

    public string searchName => circuitName.ToLower();
}
