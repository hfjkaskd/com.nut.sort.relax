using System;
using System.Collections.Generic;

namespace NutSort.Gameplay
{
    public sealed class OriginalRevokeFlow
    {
        private readonly Func<List<OriginalMoveRecord>> history;
        private readonly Func<OriginalMoveRecord,bool> revokeRecord;
        private readonly Action<int> refreshButtonState;
        private readonly Func<bool> cannotMove;
        private readonly Action fail;
        public OriginalRevokeFlow(Func<List<OriginalMoveRecord>> history,Func<OriginalMoveRecord,bool> revokeRecord,
            Action<int> refreshButtonState,Func<bool> cannotMove,Action fail)
        {
            this.history=history ?? throw new ArgumentNullException(nameof(history));
            this.revokeRecord=revokeRecord ?? throw new ArgumentNullException(nameof(revokeRecord));
            this.refreshButtonState=refreshButtonState ?? throw new ArgumentNullException(nameof(refreshButtonState));
            this.cannotMove=cannotMove ?? throw new ArgumentNullException(nameof(cannotMove));
            this.fail=fail ?? throw new ArgumentNullException(nameof(fail));
        }
        public void Revoke()
        {
            List<OriginalMoveRecord> records=history();
            if(records.Count<1)return;
            OriginalMoveRecord record=records[records.Count-1];
            if(record==null)throw new NullReferenceException("OperatorInfo");
            if(revokeRecord(record))
            {
                // Source reacquires the current history and removes the object,
                // rather than unconditionally popping the last index.
                history().Remove(record);
                refreshButtonState(2);
            }
            if(cannotMove())fail();
        }
    }
}
