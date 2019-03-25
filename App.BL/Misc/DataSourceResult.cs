using System.Collections.Generic;

namespace App.BL
{
    public class DataSourceResult<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
        public object Aggregates { get; set; }
    }
}
