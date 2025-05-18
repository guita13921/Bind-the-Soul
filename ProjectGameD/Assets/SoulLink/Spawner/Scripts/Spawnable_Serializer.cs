using System.Collections.Generic;

// (c)2022-2023 Magique Productions, Ltd. All rights reserved worldwide.

namespace Magique.SoulLink
{
    public partial class Spawnable : ISpawnableSerializer
    {
        virtual public List<string> SerializeData()
        {
            List<string> objDataList = new List<string>();

            // TODO: Serialize additional components

            // var objData = new TestData() { stringData = "This is serialized object data" };
            // objDataList.Add(JsonUtility.ToJson(objData));

            return objDataList;
        } // SerializeData()

        virtual public void DeserializeData(ExtraAIData extraAIData)
        {
            foreach (var objData in extraAIData.objectData)
            {
                // TestData objectData = JsonUtility.FromJson(objData, typeof(TestData)) as TestData;
                // Debug.Log("objectData = " + objectData.stringData);
            }
        } // DeserializeData()
    } // partial class Spawnable
} // namespace Magique.SoulLink
