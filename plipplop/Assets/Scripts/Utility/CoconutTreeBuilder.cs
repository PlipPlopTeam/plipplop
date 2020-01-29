using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class CoconutTreeBuilder : MonoBehaviour
{
   public List<GameObject> truncs;
   public List<GameObject> palms;
   public List<GameObject> coconuts;
   
   [Space]
   public int palmTreeAmount = 6;
   public int palmTreeAmountVariation = 2;
   public float palmAngleVariation = 0;
   public float palmTiltVariation;
   [Space] 
   public int coconutsAmount;
   public int coconutsAmountVariation;
   public float coconutAngleVariation = 20;
   public float coconutTiltVariation = 10;
   public float coconutHeightMin = 0.1f;
   public float coconutHeightMax = .5f;
   public float coconutDistanceFromCenterMin = .4f;
   public float coconutDistanceFromCenterMax = .4f;
   public float treeTiltVariation;

   private GameObject currentTree;

   public string path = "Assets/Prefabs/Trees/CoconutTrees/";
   

   public void CreateTree()
   {
      if (currentTree!=null)
      {
         DeleteTree();
      }

      //var r = new System.Random(15);
         
      GameObject _parent = new GameObject();
      _parent.transform.position = transform.position;
      _parent.isStatic = true;
      
      GameObject _trunc = PrefabUtility.InstantiatePrefab(PickTrunc(),_parent.transform) as GameObject;
      _trunc.isStatic = true;
      
      
      Transform _top = _trunc.transform.GetChild(0);
      _top.gameObject.isStatic = true;
      
      int _palmTreeAmount = Random.Range(palmTreeAmount - palmTreeAmountVariation, palmTreeAmount+palmTreeAmountVariation);

      List<GameObject> _palms = new List<GameObject>();

      //ADD PALMS
      for (int i = 0; i < _palmTreeAmount; i++)
      {
         _palms.Add(PrefabUtility.InstantiatePrefab(PickPalm(), _top.transform) as GameObject);

         _palms[i].transform.localEulerAngles = new Vector3(Random.Range(-palmTiltVariation, palmTiltVariation),
            (360 / _palmTreeAmount) * i + Random.Range(-palmAngleVariation / 2, palmAngleVariation / 2), 0);

         _palms[i].isStatic = true;
      }
      
      //ADD COCONUTS
      int _coconutAmount = Random.Range(coconutsAmount - coconutsAmountVariation, coconutsAmount+coconutsAmountVariation);

      List<GameObject> _coconuts = new List<GameObject>();

      for (int i = 0; i < _coconutAmount; i++)
      {
         _coconuts.Add(PrefabUtility.InstantiatePrefab(PickCoconut(), _top.transform) as GameObject);
         
         _coconuts[i].transform.localEulerAngles = new Vector3(0,
            (360 / _coconutAmount) * i + Random.Range(-coconutAngleVariation / 2, coconutAngleVariation / 2), Random.Range(0, coconutTiltVariation));
         
         _coconuts[i].transform.localPosition = new Vector3(Random.Range(coconutDistanceFromCenterMin,coconutDistanceFromCenterMax) * Mathf.Cos(_coconuts[i].transform.localEulerAngles.y),
                                                             Random.Range(-coconutHeightMin,-coconutHeightMax),
                                                         Random.Range(coconutDistanceFromCenterMin,coconutDistanceFromCenterMax)*Mathf.Sin(_coconuts[i].transform.localEulerAngles.y));

         _coconuts[i].isStatic = true;
      }

      _trunc.transform.localEulerAngles = new Vector3(Random.Range(-treeTiltVariation, treeTiltVariation),Random.Range(0f,360f),Random.Range(-treeTiltVariation, treeTiltVariation));

      _parent.name = "PalmTree";

      currentTree = _parent;
   }

   public void SaveTree()
   {
      currentTree.transform.position = Vector3.zero;
      currentTree.AddComponent<FadedApparition>();
      
      PrefabUtility.SaveAsPrefabAsset(currentTree, path + currentTree.name + "_"+GetAvailableTreeName(currentTree.name) + ".prefab");
   }
   


   string GetAvailableTreeName(string name)
   {
      var id = Guid.NewGuid().ToString().Split('-')[0];
      while (File.Exists(path+name+"_"+id+".prefab"))
      {
         id = Guid.NewGuid().ToString().Split('-')[0];
      }

      return id;
   }

   public void DeleteTree()
   {
      DestroyImmediate(currentTree);
      currentTree = null;
   }

   GameObject PickTrunc()
   {
      return truncs[Random.Range(0,truncs.Count)];
   }

   GameObject PickPalm()
   {
      return palms[Random.Range(0,palms.Count)]; ;
   }

   GameObject PickCoconut()
   {
      return coconuts[Random.Range(0, coconuts.Count)];
   }
}
