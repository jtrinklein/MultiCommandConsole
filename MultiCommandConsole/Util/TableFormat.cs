namespace MultiCommandConsole.Util
{
    public class TableFormat
    {
        public string Spacer4Header { get; set; }
        public string Spacer4FirstRow { get; set; }
        public string Spacer4OtherRows { get; set; }

        public bool AutoAlignWidth { get; set; }
        public int[] Widths { get; set; }
    }
}