using System.Collections;
using UnityEngine;

public class MakeWet : MonoBehaviour
{
    private MeshRenderer[] rendererArray;

    private void Start()
    {
        AddWetMat(this.transform);
    }

    private void OnDisable()
    {
        EventHandler.eventHandler.RTSWeatherChange -= SwitchWetness;
    }

    public void AddWetMat(Transform transform)
    {
        rendererArray = transform.gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rendererArray.Length; i++)
        {
            Material[] tempMatArray = rendererArray[i].materials;
            Material wetMat = Material.Instantiate(Resources.Load<Material>("WetMatBase"));
            tempMatArray[0].EnableKeyword("_NORMALMAP");
            wetMat.SetTexture("_NormalMap", tempMatArray[0].GetTexture("_BumpMap"));
            Material[] tempMatArray2 = new Material[] { tempMatArray[0], wetMat };
            rendererArray[i].materials = tempMatArray2;
            rendererArray[i].materials[rendererArray[i].materials.Length - 1].SetFloat("_Wetness", 0);
            EventHandler.eventHandler.RTSWeatherChange += SwitchWetness;
        }
    }

    private void SwitchWetness(int weatherIndex)
    {
        if (weatherIndex == 0)
        {
            StartCoroutine(DecreaseWetness());
        }
        else if (weatherIndex == 1)
        {
            StartCoroutine(IncreaseWetness());
        }
    }

    private IEnumerator IncreaseWetness()
    {
        float timer = 10;
        for (int r = 0; r < rendererArray.Length; r++)
        {
            for (float i = 0; i < timer; i += Time.deltaTime)
            {
                rendererArray[r].materials[rendererArray[r].materials.Length - 1].SetFloat("_Wetness", i);
                rendererArray[r].materials[rendererArray[r].materials.Length - 1].SetFloat("_Metallic", 1 - (i * 0.1f));
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator DecreaseWetness()
    {
        for (int r = 0; r < rendererArray.Length; r++)
        {
            for (float i = 10; i > 0; i -= Time.deltaTime)
            {
                rendererArray[r].materials[rendererArray[r].materials.Length - 1].SetFloat("_Wetness", i);
                rendererArray[r].materials[rendererArray[r].materials.Length - 1].SetFloat("_Metallic", 1 - (i * 0.1f));
                yield return new WaitForEndOfFrame();
            }
        }
    }
}