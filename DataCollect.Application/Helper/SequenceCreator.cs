using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Application.Helper
{
    public class SequenceCreator
    {
        private static int _squenceNo;
        public static string GetIdBySeqNoAndKey(string key) => key + Guid.NewGuid().ToString("N");

        public static int GetSequenceNo()
        {
            if (_squenceNo >= 2147483647)
            {
                _squenceNo = 1;
            }
            else
            {
                _squenceNo = _squenceNo + 1;
            }

            return _squenceNo;
        }

        public static int GetSequenceNoForClient()
        {
            if (_squenceNo >= 2147483647)
            {
                _squenceNo = 1;
            }
            else
            {
                _squenceNo = _squenceNo + 1;
            }

            return _squenceNo;
        }



        public static string GetIdBySequenceNo()
        {
            var sequenceNo = GetSequenceNo();
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + sequenceNo.ToString("d5");
        }

        public static string GetVipIdBySeqNoAndKey(string key) => key + GetIdBySequenceNo();
    }
}
