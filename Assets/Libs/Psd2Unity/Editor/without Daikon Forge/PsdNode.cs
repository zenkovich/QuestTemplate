using System.Collections;
using System.Collections.Generic;

public class PsdNode {

    public string name;
    public string fileName;
    public float width, height, left, top;
    public Stack<PsdNode> children = new Stack<PsdNode>();

    public string AssetPath (string baseAssetPath) {
        return baseAssetPath + "/" + fileName;
    }

}
