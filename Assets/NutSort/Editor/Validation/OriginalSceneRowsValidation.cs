using System;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalSceneRowsValidation
    {
        public static void Validate(OriginalLevelRepository repository)
        {
            var root=new GameObject("Row hierarchy fixture");
            try
            {
                var pool=root.AddComponent<OriginalPrefabPool>();
                var holder=new GameObject("ScenePosGroup");holder.transform.SetParent(root.transform,false);
                var group=holder.AddComponent<OriginalScenePosGroup>();
                var layout=Resources.Load<OriginalLayoutSettings>("Configuration/OriginalLayout");
                var settings=Resources.Load<OriginalScrewSettings>("Configuration/OriginalScrew");
                holder.transform.SetPositionAndRotation(new Vector3(7,2,-4),Quaternion.Euler(0,30,0));holder.transform.localScale=Vector3.one*2;
                group.Bind(pool,layout,settings,11);
                Check(holder.transform.childCount==3,"Actual three row parents");
                for(int i=0;i<11;i++)
                {
                    var pos=group.GetPosition(i);var coordinate=layout.Coordinate(11,i);
                    Check(pos.name=="Pos_"+coordinate.y&&pos.parent.name=="Row_"+coordinate.x,"Native row and column names");
                    var metadata=pos.GetComponent<OriginalScrewPosition>();
                    Check(metadata.Row==coordinate.x&&metadata.Column==coordinate.y&&metadata.Index==i,"Native position component metadata");
                    Near(pos.position,layout.Position(11,i),"Source world-preserving parenting under transformed group");
                    Check(Mathf.Abs(pos.localPosition.z)<.00001f,"Z belongs to row only");
                    Near(pos.parent.position,layout.RowPosition(3,coordinate.x),"Row world offset");
                }
                group.Clear();Check(holder.transform.childCount==0,"Position and row objects leave group on clear");
                var data=repository.LoadBoard(false,"4b56d_1_1-1");var first=data.B[0];data.B=new[]{first,first,first,first,first,first};
                var board=new OriginalBoardState(data,layout);
                group.Bind(pool,layout,settings,board);
                var old=group.GetPosition(0);var firstRow=old.parent;
                firstRow.SetAsLastSibling();
                old.SetAsLastSibling(); // Column identity must survive sibling changes as well.
                board.AppendNullScrew(()=>false);var added=group.Append(board,layout,settings);
                Check(added.parent==firstRow&&board.Screws[6].Coordinate==new Vector2Int(0,3),"Tie follows changed actual row sibling order");
                Near(old.localPosition,layout.ColumnPosition(4,0),"Recenter by Pos name despite changed child sibling order");
                Check(group.GetPosition(0)==old&&firstRow.childCount==4,"Existing objects retained during append");
                group.Clear();group.Bind(pool,layout,settings,5);
                Check(holder.transform.childCount==1&&group.GetPosition(4).parent.childCount==5,"Pooled rows contain no previous positions");
                group.Clear();
                Debug.Log("NUT_SCENE_ROWS_VALIDATION_PASS prefab Row/Pos hierarchy, native names and world-preserving parenting, named-column recenter, actual sibling tie ordering, stable positions and pool clear/rebind.");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static void Near(Vector3 a,Vector3 b,string message){Check((a-b).sqrMagnitude<.000001f,message);}
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
