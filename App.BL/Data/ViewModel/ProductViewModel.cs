namespace App.BL.Data.ViewModel
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierContactName { get; set; }
        public string SupplierContactTitle { get; set; }
        public string SupplierAddress { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public byte[] CategoryPicture { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    }
}
