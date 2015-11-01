using System;

namespace RawLauncherWPF.Defreezer
{
    public sealed class UnitRef
    {
        public static readonly byte[] SearchPattern = {0x80, 0xB0, 0x04, 0x00, 0x00, 0xD1, 0x00, 0x00, 0x00, 0x02, 0x04};
        private byte[] _firstId;

        public UnitRef(int pos, byte[] fId, byte[] sId)
        {
            if (fId.Length != 3 || sId.Length != 4)
                throw new Exception("Defreezer: Wrong Array Lenght");
            Position = pos;
            _firstId = fId;
            IntId = _firstId[0] + (_firstId[1] << 8) + (_firstId[2] << 16);
            CompleteId = new byte[] {fId[0], fId[1], fId[2], 0x00, 0x01, 0x04, sId[0], sId[1], sId[2], sId[3]};
        }

        public byte[] CompleteId { get; }

        public byte[] FirstId
        {
            get { return _firstId; }
            set
            {
                if (value.Length > 3)
                    for (int i = 0; i < 3; i++)
                        _firstId[i] = value[i];
                else
                    _firstId = value;
            }
        }

        public int IntId { get; set; }

        public int Position { get; set; }
    }
}