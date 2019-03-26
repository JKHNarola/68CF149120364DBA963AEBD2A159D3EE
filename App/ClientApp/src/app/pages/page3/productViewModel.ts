export class ProductViewModel {
    ProductID: number;
    ProductName: string;
    SupplierID?: number;
    SupplierCompanyName: string;
    SupplierContactName: string;
    SupplierContactTitle: string;
    SupplierAddress: string;
    CategoryID?: number;
    CategoryName: string;
    CategoryPicture: Array<number>;
    QuantityPerUnit: string;
    UnitPrice?: number;
    UnitsInStock?: number;
    UnitsOnOrder?: number;
    ReorderLevel?: number;
    Discontinued: boolean;
}
