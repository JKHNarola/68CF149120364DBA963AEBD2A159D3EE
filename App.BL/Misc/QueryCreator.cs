using System.Collections.Generic;

namespace App.BL
{
    public class Q
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Dictionary<string, object> Extras { get; set; }
        public List<ConditionPart> WhereClause { get; set; } = new List<ConditionPart>();
        public Sort Sort { get; set; }
    }

    public class Sort
    {
        public string ColumnName { get; set; }
        public SortOrder Direction { get; set; }
    }

    public class ConditionPart
    {
        public bool IsStartBracket { get; set; }
        public bool IsEndBracket { get; set; }
        public string ColumnName { get; set; }
        public Operator? Operator { get; set; }
        public object Value { get; set; }
    }
}
