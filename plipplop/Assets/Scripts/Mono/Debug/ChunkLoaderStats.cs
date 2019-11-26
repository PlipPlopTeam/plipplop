using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;

public class ChunkLoaderStats : MonoBehaviour
{
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = new StringBuilder()
            .AppendLine(string.Format("Prop count: {0}", Game.i.chunkLoader.GetPropCount()))
            .AppendLine(string.Format("Storage size: {0}", Game.i.chunkLoader.GetStorageCount()))
            .AppendLine(
                string.Format("Storage per chunk:\n{0}",
                    string.Join("\n", Game.i.chunkLoader.GetStoragesCount().Select(x => string.Format("Storage in {0}: {1}", x.Key, x.Value)))
                )
            )
            .ToString();
    }
}
