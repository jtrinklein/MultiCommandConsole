using System.Collections.Generic;

namespace MultiCommandConsole.Commands
{
    public class BurstSegment
    {
        public int Send;
        public int Wait;
        public int Repeat = 1;

        public override string ToString()
        {
            return string.Format("s{0}w{1}r{2}", Send, Wait, Repeat);
        }

        public static IList<BurstSegment> ParseSegments(string burstPattern)
        {
            var segments = new List<BurstSegment>();
            int sindex = -1, windex = 0, rindex = 0;

            for (int index = 0; index < burstPattern.Length; index++)
            {
                var c = burstPattern[index];
                switch (c)
                {
                    case 's':
                        if (sindex >= 0)
                        {
                            segments.Add(Parse(burstPattern, index, sindex, windex, rindex));
                        }
                        sindex = index;
                        break;
                    case 'w':
                        windex = index;
                        break;
                    case 'r':
                        rindex = index;
                        break;
                }
            }
            segments.Add(Parse(burstPattern, burstPattern.Length, sindex, windex, rindex));
            return segments;
        }

        private static BurstSegment Parse(string burstPattern, int index, int sindex, int windex, int rindex)
        {
            bool hasWait = windex > sindex;
            bool hasRepeat = rindex > sindex;

            var segment = new BurstSegment();
            var substring = burstPattern.Substring(sindex + 1, (hasWait ? windex : hasRepeat ? rindex : index) - sindex - 1);
            segment.Send = int.Parse(substring);
            if (hasWait)
            {
                substring = burstPattern.Substring(windex + 1, (hasRepeat ? rindex : index) - windex - 1);
                segment.Wait = int.Parse(substring);
            }
            if (hasRepeat)
            {
                substring = burstPattern.Substring(rindex + 1, index - rindex - 1);
                segment.Repeat = int.Parse(substring);
            }
            return segment;
        }
    }
}