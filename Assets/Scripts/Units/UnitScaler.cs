using UnityEngine;

namespace UnfrozenTestWork
{
    public class UnitScaler
    {
        public void ScaleUnits(UnitModel[] units, Rect unitsRect, Rect fitInto)
        {
            float scaleX = fitInto.width < unitsRect.width ? fitInto.width / unitsRect.width : 1;
            float scaleY = fitInto.height < unitsRect.height ? fitInto.height / unitsRect.height : 1;
            float minScale = scaleX < scaleY ? scaleX : scaleY;

            for (int i = 0; i < units.Length; i++)
            {
                units[i].transform.localScale = Vector3.one * minScale;
                var hpBar = units[i].GetComponentInChildren<HealthBar>();
                //hpBar.transform.localScale *= minScale;
                var initiativeBar = units[i].GetComponentInChildren<InitiativeBar>();
                //initiativeBar.transform.localScale *= minScale;
                var selection = units[i].GetComponentInChildren<UnitSelectionDisplayer>();
                //selection.transform.localScale *= minScale;
            }
        }
    }
}
