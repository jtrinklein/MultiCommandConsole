using System.Linq;

namespace MultiCommandConsole.Util
{
    public class TableFormat
    {
        public string Indent { get; set; }
        public string Spacer4Header { get; set; }
        public string Spacer4FirstRow { get; set; }
        public string Spacer4OtherRows { get; set; }

        /// <summary>
        /// The widths for each column. 
        /// Any value less than 0 will result in scanning all rows to find the maximum width.
        /// If the width is to large for the current screen, the rows to the right will be trimmed and dropped.
        /// </summary>
        public int[] Widths { get; set; }

        public TableFormat()
        {
            Indent = " ";
            Spacer4Header = "  ";
            Spacer4FirstRow = ": ";
            Spacer4OtherRows = "  ";
            Widths = new int[0];
        }

        public TableFormat Clone()
        {
            return new TableFormat
                {
                    Indent = Indent,
                    Spacer4Header = Spacer4Header,
                    Spacer4FirstRow = Spacer4FirstRow,
                    Spacer4OtherRows = Spacer4OtherRows,
                    Widths = Widths
                };
        }
    }
}